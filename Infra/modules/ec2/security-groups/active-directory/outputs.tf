output "domain_controller_id" {
  description = "Domain Controller Security Group ID"
  value       = aws_security_group.domain_controller.id
}

output "domain_member_id" {
  description = "Domain member Security Group ID"
  value       = aws_security_group.domain_member.id
}

output "inter_dc_id" {
  description = "Inter Domain Controller Security Group ID"
  value       = aws_security_group.inter_dc.id
}
