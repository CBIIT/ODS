output "id" {
  description = "Transit Gateway ID"
  value       = aws_ec2_transit_gateway.tgw.id
}

output "arn" {
  description = "Transit Gateway ARN"
  value       = aws_ec2_transit_gateway.tgw.arn
}

output "association_default_route_table_id" {
  description = "Transit Gateway Default Route Table ID"
  value = aws_ec2_transit_gateway.tgw.association_default_route_table_id
}
