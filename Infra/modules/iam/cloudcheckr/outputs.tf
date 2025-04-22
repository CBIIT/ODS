output "role_arn" {
  description = "ARN of CloudCheckr role"
  value       = aws_iam_role.privo_amb.arn
}

output "role_name" {
  description = "Name of the CloudCheckr role"
  value       = aws_iam_role.privo_amb.name
}
