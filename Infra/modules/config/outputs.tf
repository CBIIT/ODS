output "config_role_arn" {
  description = "Config service-linked role ARN."
  value       = aws_iam_service_linked_role.config.arn
}

output "sns_arn" {
  description = "SNS topic ARN."
  value       = aws_sns_topic.topic.arn
}
