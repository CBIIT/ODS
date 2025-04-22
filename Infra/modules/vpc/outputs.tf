output "id" {
  value       = aws_vpc.vpc.id
  description = "VPC ID"
}

output "cidr" {
  value       = aws_vpc.vpc.cidr_block
  description = "VPC CIDR"
}

#
# NAT Gateways
#

output "nat_gateway_ids" {
  value       = { for k, v in aws_nat_gateway.nat_gw : k => v.id }
  description = "NAT Gateway IDs"
}

output "nat_gateway_public_ips" {
  value       = { for k, v in aws_nat_gateway.nat_gw : k => v.public_ip }
  description = "NAT Gateway IDs"
}

#
# Public Subnets
#

output "public_network_acl" {
  value       = { for k, v in aws_network_acl.public : k => v.id }
  description = "Public subnet NACLs"
}

output "public_route_tables" {
  value       = { for k, v in aws_route_table.public : k => v.id }
  description = "Public subnet route tables"
}

output "public_subnet_ids" {
  value       = { for k, v in aws_subnet.public : k => v.id }
  description = "Public subnet IDs"
}

output "public_subnet_cidrs" {
  value       = { for k, v in aws_subnet.public : k => v.cidr_block }
  description = "Public subnet CIDRs"
}

#
# Private subnets
#

output "private_network_acl" {
  value       = { for k, v in aws_network_acl.private : k => v.id }
  description = "Private subnet NACLs"
}

output "private_route_tables" {
  value       = { for k, v in aws_route_table.private : k => v.id }
  description = "Private subnet route tables"
}

output "private_subnet_ids" {
  value       = { for k, v in aws_subnet.private : k => v.id }
  description = "Private subnet IDs"
}

output "private_subnet_cidrs" {
  value       = { for k, v in aws_subnet.private : k => v.cidr_block }
  description = "Private subnet CIDRs"
}

#
# Data subnets
#

output "data_network_acl" {
  value       = { for k, v in aws_network_acl.data : k => v.id }
  description = "Data subnet NACLs"
}

output "data_route_tables" {
  value       = { for k, v in aws_route_table.data : k => v.id }
  description = "Data subnet route tables"
}

output "data_subnet_ids" {
  value       = { for k, v in aws_subnet.data : k => v.id }
  description = "Data subnet IDs"
}

output "data_subnet_cidrs" {
  value       = { for k, v in aws_subnet.data : k => v.cidr_block }
  description = "Data subnet CIDRs"
}
