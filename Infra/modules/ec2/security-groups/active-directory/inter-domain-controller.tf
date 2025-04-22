resource "aws_security_group" "inter_dc" {
  name        = "inter-domain-controller"
  description = "Communication between Domain Controllers"
  vpc_id      = var.vpc_id
  tags        = merge(var.tags, { "Name" = "inter-domain-controller" })
}

resource "aws_security_group_rule" "ingress_inter_dc_full" {
  for_each          = toset(var.corporate_domain_controller_cidrs)
  type              = "ingress"
  description       = "full igress"
  from_port         = 0
  to_port           = 0
  protocol          = "all"
  cidr_blocks       = [each.key]
  security_group_id = aws_security_group.inter_dc.id
}

resource "aws_security_group_rule" "ingress_inter_dc_self_full" {
  type              = "ingress"
  description       = "full igress"
  from_port         = 0
  to_port           = 0
  protocol          = "all"
  self              = true
  security_group_id = aws_security_group.inter_dc.id
}

resource "aws_security_group_rule" "engress_inter_dc_full" {
  for_each          = toset(var.corporate_domain_controller_cidrs)
  type              = "egress"
  description       = "full egress"
  from_port         = 0
  to_port           = 0
  protocol          = "all"
  cidr_blocks       = [each.key]
  security_group_id = aws_security_group.inter_dc.id
}

resource "aws_security_group_rule" "engress_inter_dc_self_full" {
  type              = "egress"
  description       = "full egress"
  from_port         = 0
  to_port           = 0
  protocol          = "all"
  self              = true
  security_group_id = aws_security_group.inter_dc.id
}
