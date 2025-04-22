output "corp_vpn_id" {
  description = "Corporate VPN security group ID"
  value       = var.corporate_cidr != "" ? aws_security_group.corp_all[0].id : null
}

output "icmp_id" {
  description = "ICMP from all security group ID"
  value       = aws_security_group.icmp_all.id
}

output "vpc_id" {
  description = "Local VPC security group ID"
  value       = aws_security_group.vpc_all.id
}
