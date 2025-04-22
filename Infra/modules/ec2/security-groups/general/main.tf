# Allow ICMP from all

resource "aws_security_group" "icmp_all" {
  name        = "icmp-all"
  description = "Allows ICMP to/from ALL"
  vpc_id      = var.vpc_id
  tags        = merge(var.tags, { "Name" = "icmp-all" })
}

resource "aws_security_group_rule" "ingress_icmp_all" {
  type              = "ingress"
  description       = "ICMP"
  to_port           = -1
  from_port         = -1
  protocol          = "icmp"
  cidr_blocks       = ["0.0.0.0/0"]
  security_group_id = aws_security_group.icmp_all.id
}

resource "aws_security_group_rule" "egress_icmp_full" {
  type              = "egress"
  description       = "full ICMP egress to ALL"
  from_port         = -1
  to_port           = -1
  protocol          = "icmp"
  cidr_blocks       = ["0.0.0.0/0"]
  security_group_id = aws_security_group.icmp_all.id
}

# Allow all traffic to and from the VPC CIDR

resource "aws_security_group" "vpc_all" {
  name        = "vpc-all"
  description = "Allow all traffic to and from the VPC CIDR"
  vpc_id      = var.vpc_id
  tags        = merge(var.tags, { "Name" = "vpc-all" })
}

resource "aws_security_group_rule" "ingress_vpc_all" {
  type              = "ingress"
  description       = "vpc"
  to_port           = -1
  from_port         = -1
  protocol          = "all"
  cidr_blocks       = [var.vpc_cidr]
  security_group_id = aws_security_group.vpc_all.id
}

resource "aws_security_group_rule" "egress_vpc_full" {
  type              = "egress"
  description       = "full egress to VPC"
  from_port         = -1
  to_port           = -1
  protocol          = "all"
  cidr_blocks       = [var.vpc_cidr]
  security_group_id = aws_security_group.vpc_all.id
}

# Allow all traffic to and from the corporate VPN CIDR

resource "aws_security_group" "corp_all" {
  count       = var.corporate_cidr != "" ? 1 : 0
  name        = "corp-all"
  description = "Allow all traffic to and from the Corporate VPN CIDR"
  vpc_id      = var.vpc_id
  tags        = merge(var.tags, { "Name" = "corp-all" })
}

resource "aws_security_group_rule" "ingress_corp_all" {
  count             = var.corporate_cidr != "" ? 1 : 0
  type              = "ingress"
  description       = "Corporate VPN"
  to_port           = -1
  from_port         = -1
  protocol          = "all"
  cidr_blocks       = [var.corporate_cidr]
  security_group_id = aws_security_group.corp_all[0].id
}

resource "aws_security_group_rule" "egress_corp_full" {
  count             = var.corporate_cidr != "" ? 1 : 0
  type              = "egress"
  description       = "full egress to Corporate VPN"
  from_port         = -1
  to_port           = -1
  protocol          = "all"
  cidr_blocks       = [var.corporate_cidr]
  security_group_id = aws_security_group.corp_all[0].id
}
