resource "aws_security_group" "domain_controller" {
  name        = "domain-controller"
  description = "Manage access to Active Directory instances"
  vpc_id      = var.vpc_id
  tags        = merge(var.tags, { "Name" = "domain-controller" })
}

resource "aws_security_group_rule" "ingress_dc_vpc_53_tcp" {
  type              = "ingress"
  description       = "DNS"
  from_port         = 53
  to_port           = 53
  protocol          = "tcp"
  cidr_blocks       = [var.vpc_cidr]
  security_group_id = aws_security_group.domain_controller.id
}

resource "aws_security_group_rule" "ingress_dc_vpc_53_udp" {
  type              = "ingress"
  description       = "DNS"
  from_port         = 53
  to_port           = 53
  protocol          = "udp"
  cidr_blocks       = [var.vpc_cidr]
  security_group_id = aws_security_group.domain_controller.id
}

resource "aws_security_group_rule" "ingress_dc_vpc_icmp" {
  type              = "ingress"
  description       = "ICMP"
  from_port         = -1
  to_port           = -1
  protocol          = "icmp"
  cidr_blocks       = [var.vpc_cidr]
  security_group_id = aws_security_group.domain_controller.id
}

resource "aws_security_group_rule" "ingress_dc_domain_member_tcp_88" {
  type                     = "ingress"
  description              = "Kerberos"
  from_port                = 88
  to_port                  = 88
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.domain_member.id
  security_group_id        = aws_security_group.domain_controller.id
}

resource "aws_security_group_rule" "ingress_dc_domain_member_udp_88" {
  type                     = "ingress"
  description              = "Kerberos"
  from_port                = 88
  to_port                  = 88
  protocol                 = "udp"
  source_security_group_id = aws_security_group.domain_member.id
  security_group_id        = aws_security_group.domain_controller.id
}

resource "aws_security_group_rule" "ingress_dc_domain_member_udp_123" {
  type                     = "ingress"
  description              = "W32Time"
  from_port                = 123
  to_port                  = 123
  protocol                 = "udp"
  source_security_group_id = aws_security_group.domain_member.id
  security_group_id        = aws_security_group.domain_controller.id
}

resource "aws_security_group_rule" "ingress_dc_domain_member_tcp_135" {
  type                     = "ingress"
  description              = "RPC, EPM"
  from_port                = 135
  to_port                  = 135
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.domain_member.id
  security_group_id        = aws_security_group.domain_controller.id
}

resource "aws_security_group_rule" "ingress_dc_domain_member_udp_137" {
  type                     = "ingress"
  description              = "NetBIOS Name Resolution"
  from_port                = 137
  to_port                  = 137
  protocol                 = "udp"
  source_security_group_id = aws_security_group.domain_member.id
  security_group_id        = aws_security_group.domain_controller.id
}

resource "aws_security_group_rule" "ingress_dc_domain_member_udp_138" {
  type                     = "ingress"
  description              = "NetBIOS Datagram Service"
  from_port                = 138
  to_port                  = 138
  protocol                 = "udp"
  source_security_group_id = aws_security_group.domain_member.id
  security_group_id        = aws_security_group.domain_controller.id
}

resource "aws_security_group_rule" "ingress_dc_domain_member_tcp_139" {
  type                     = "ingress"
  description              = "NetBIOS Session Service"
  from_port                = 139
  to_port                  = 139
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.domain_member.id
  security_group_id        = aws_security_group.domain_controller.id
}

resource "aws_security_group_rule" "ingress_dc_domain_member_tcp_389" {
  type                     = "ingress"
  description              = "LDAP"
  from_port                = 389
  to_port                  = 389
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.domain_member.id
  security_group_id        = aws_security_group.domain_controller.id
}

resource "aws_security_group_rule" "ingress_dc_domain_member_udp_389" {
  type                     = "ingress"
  description              = "LDAP"
  from_port                = 389
  to_port                  = 389
  protocol                 = "udp"
  source_security_group_id = aws_security_group.domain_member.id
  security_group_id        = aws_security_group.domain_controller.id
}

resource "aws_security_group_rule" "ingress_dc_domain_member_tcp_445" {
  type                     = "ingress"
  description              = "SMB"
  from_port                = 445
  to_port                  = 445
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.domain_member.id
  security_group_id        = aws_security_group.domain_controller.id
}

resource "aws_security_group_rule" "ingress_dc_domain_member_tcp_464" {
  type                     = "ingress"
  description              = "Kerberos Password V5"
  from_port                = 464
  to_port                  = 464
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.domain_member.id
  security_group_id        = aws_security_group.domain_controller.id
}

resource "aws_security_group_rule" "ingress_dc_domain_member_udp_464" {
  type                     = "ingress"
  description              = "Kerberos Password V5"
  from_port                = 464
  to_port                  = 464
  protocol                 = "udp"
  source_security_group_id = aws_security_group.domain_member.id
  security_group_id        = aws_security_group.domain_controller.id
}

resource "aws_security_group_rule" "ingress_dc_domain_member_tcp_636" {
  type                     = "ingress"
  description              = "LDAP SSL"
  from_port                = 636
  to_port                  = 636
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.domain_member.id
  security_group_id        = aws_security_group.domain_controller.id
}

resource "aws_security_group_rule" "ingress_dc_domain_member_tcp_3268" {
  type                     = "ingress"
  description              = "LDAP GC"
  from_port                = 3268
  to_port                  = 3268
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.domain_member.id
  security_group_id        = aws_security_group.domain_controller.id
}

resource "aws_security_group_rule" "ingress_dc_domain_member_tcp_3269" {
  type                     = "ingress"
  description              = "LDAP GC SSL"
  from_port                = 3269
  to_port                  = 3269
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.domain_member.id
  security_group_id        = aws_security_group.domain_controller.id
}

resource "aws_security_group_rule" "ingress_dc_domain_member_tcp_5722" {
  type                     = "ingress"
  description              = "RPC, DFSR (SYSVOL)"
  from_port                = 5722
  to_port                  = 5722
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.domain_member.id
  security_group_id        = aws_security_group.domain_controller.id
}

resource "aws_security_group_rule" "ingress_dc_domain_member_tcp_9389" {
  type                     = "ingress"
  description              = "SOAP"
  from_port                = 9389
  to_port                  = 9389
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.domain_member.id
  security_group_id        = aws_security_group.domain_controller.id
}

resource "aws_security_group_rule" "ingress_dc_domain_member_tcp_ephemeral" {
  type                     = "ingress"
  description              = "Ephemeral Port Range"
  from_port                = 49152
  to_port                  = 65535
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.domain_member.id
  security_group_id        = aws_security_group.domain_controller.id
}

resource "aws_security_group_rule" "ingress_dc_domain_member_udp_ephemeral" {
  type                     = "ingress"
  description              = "Ephemeral Port Range"
  from_port                = 49152
  to_port                  = 65535
  protocol                 = "udp"
  source_security_group_id = aws_security_group.domain_member.id
  security_group_id        = aws_security_group.domain_controller.id
}

resource "aws_security_group_rule" "ingress_dc_domain_member_icmp" {
  type                     = "ingress"
  description              = "ICMP"
  from_port                = -1
  to_port                  = -1
  protocol                 = "icmp"
  source_security_group_id = aws_security_group.domain_member.id
  security_group_id        = aws_security_group.domain_controller.id
}

resource "aws_security_group_rule" "domain_controller_egress_full" {
  type              = "egress"
  description       = "full egress"
  from_port         = 0
  to_port           = 0
  protocol          = "all"
  cidr_blocks       = ["0.0.0.0/0"]
  security_group_id = aws_security_group.domain_controller.id
}
