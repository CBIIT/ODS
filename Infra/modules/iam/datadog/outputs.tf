output "name" {
  description = "The name of the datadog integration AWS IAM Role"
  value       = aws_iam_role.datadog_aws_integration.name
}