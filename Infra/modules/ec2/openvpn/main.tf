data "aws_region" "current" {}

data "aws_subnet" "selected" {
  id = var.subnet_id
}

resource "aws_eip" "eip" {
  vpc = true
}

resource "aws_eip_association" "openvpn" {
  instance_id   = aws_instance.openvpn.id
  allocation_id = aws_eip.eip.id
}

resource "aws_instance" "openvpn" {
  ami                     = var.ami
  instance_type           = var.instance_type
  disable_api_termination = var.disable_api_termination
  iam_instance_profile    = aws_iam_instance_profile.openvpn.id
  key_name                = var.key_name
  subnet_id               = var.subnet_id
  vpc_security_group_ids  = [aws_security_group.openvpn.id]
  root_block_device {
    volume_type = "gp2"
    volume_size = var.root_volume_size
    encrypted   = var.ebs_encryption
    kms_key_id  = var.ebs_kms_key_id
  }

  user_data = <<USER_DATA
#!/bin/bash
hostnamectl set-hostname ${var.name}
yum -y update
# AWS Systems Manager (SSM) Agent
yum -y install https://ec2-downloads-windows.s3.amazonaws.com/SSMAgent/latest/linux_amd64/amazon-ssm-agent.rpm
sed -i s/us\-east\-1/${data.aws_region.current.name}/g /etc/awslogs/awscli.conf
systemctl restart awslogsd.service
# OpenVPN
yum -y install lzo ncurses-compat-libs
yum -y install https://as-repository.openvpn.net/as-repo-centos7.rpm
yum -y install openvpn-as
echo "export PATH=$PATH:/usr/local/openvpn_as/scripts" > /etc/profile.d/openvpn.sh
source /etc/profile.d/openvpn.sh
sacli stop
sacli --key "host.name" --value "${aws_eip.eip.public_ip}" ConfigPut
sacli --key "vpn.client.routing.reroute_dns" --value "false" ConfigPut
sacli --key "vpn.client.routing.reroute_gw" --value "false" ConfigPut
sacli --key "vpn.server.routing.private_network.0" --value "${var.vpc_cidr}" ConfigPut
sacli start
sacli ConfigQuery
USER_DATA

  volume_tags = merge(var.tags, { "Name" = "${var.name} root volume" })
  tags        = merge(var.tags, { "Name" = var.name, "ssm" = var.enable_ssm })


}

resource "aws_iam_role" "openvpn" {
  name        = var.name
  path        = "/"
  description = "${var.name} EC2 instance"

  assume_role_policy = <<POLICY
{
    "Version": "2012-10-17",
    "Statement": [
      {
        "Effect": "Allow",
        "Principal": {
          "Service": "ec2.amazonaws.com"
        },
        "Action": "sts:AssumeRole"
      }
    ]
}
POLICY
}

resource "aws_iam_instance_profile" "openvpn" {
  name = aws_iam_role.openvpn.name
  role = aws_iam_role.openvpn.name
}

resource "aws_iam_role_policy_attachment" "openvpn" {
  role       = aws_iam_role.openvpn.name
  policy_arn = "arn:aws:iam::aws:policy/AmazonSSMManagedInstanceCore"
}

#
# Security group, allow TCP 443 and UDP 1194 from var.vpn_ingress_cidr.
# TCP 22 and TCP 943 (OpenVPN Web Admin) from inside the VPC.
#

resource "aws_security_group" "openvpn" {
  name_prefix = "${var.name}-"
  description = "Internal and external ${var.name} instance access"
  vpc_id      = data.aws_subnet.selected.vpc_id

  tags = merge(var.tags, { "Name" = var.name })
}

resource "aws_security_group_rule" "ingress_ssh" {
  type              = "ingress"
  from_port         = 22
  to_port           = 22
  protocol          = "tcp"
  cidr_blocks       = [var.vpc_cidr]
  security_group_id = aws_security_group.openvpn.id
  description       = "SSH"
}

resource "aws_security_group_rule" "ingress_https" {
  type              = "ingress"
  from_port         = 443
  to_port           = 443
  protocol          = "tcp"
  cidr_blocks       = [var.vpn_ingress_cidr]
  security_group_id = aws_security_group.openvpn.id
  description       = "OpenVPN HTTPS"
}

resource "aws_security_group_rule" "ingress_https_admin" {
  type              = "ingress"
  from_port         = 943
  to_port           = 943
  protocol          = "tcp"
  cidr_blocks       = [var.vpc_cidr]
  security_group_id = aws_security_group.openvpn.id
  description       = "OpenVPN HTTPS Admin Management"
}

resource "aws_security_group_rule" "ingress_vpn" {
  type              = "ingress"
  from_port         = 1194
  to_port           = 1194
  protocol          = "udp"
  cidr_blocks       = [var.vpn_ingress_cidr]
  security_group_id = aws_security_group.openvpn.id
  description       = "OpenVPN UDP"
}

resource "aws_security_group_rule" "egress_full" {
  type              = "egress"
  from_port         = 0
  to_port           = 0
  protocol          = "-1"
  cidr_blocks       = ["0.0.0.0/0"]
  security_group_id = aws_security_group.openvpn.id
  description       = "ALL"
}
