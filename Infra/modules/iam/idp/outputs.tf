output provider_arn {
  description = "The ARN of the IAM Identity Provider being created"
  value       = aws_iam_saml_provider.idp.arn
}
