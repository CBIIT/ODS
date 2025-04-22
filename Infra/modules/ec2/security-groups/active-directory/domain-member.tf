resource "aws_security_group" "domain_member" {
  name        = "domain-member"
  description = "Active Directory Domain Member"
  vpc_id      = var.vpc_id
  tags        = merge(var.tags, { "Name" = "domain-member" })
}

resource "aws_security_group_rule" "ingress_domain_member_egress_full" {
  type              = "egress"
  description       = "full egress"
  from_port         = 0
  to_port           = 0
  protocol          = "all"
  cidr_blocks       = ["0.0.0.0/0"]
  security_group_id = aws_security_group.domain_member.id
}
