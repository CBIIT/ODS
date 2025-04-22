output "backup_key_arn" {
  description = "ARN of the backup key"
  value       = var.backup ? aws_kms_key.backup[0].arn : null
}

output "backup_key_id" {
  description = "ID of the backup key"
  value       = var.backup ? aws_kms_key.backup[0].id : null
}

output "dynamodb_key_arn" {
  description = "ARN of the Dynamodb key"
  value       = var.dynamodb ? aws_kms_key.dynamodb[0].arn : null
}

output "dynamodb_key_id" {
  description = "ID of the Dynamodb key"
  value       = var.dynamodb ? aws_kms_key.dynamodb[0].id : null
}

output "ebs_key_arn" {
  description = "ARN of the EBS key"
  value       = var.ebs ? aws_kms_key.ebs[0].arn : null
}

output "ebs_key_id" {
  description = "ID of the EBS key"
  value       = var.ebs ? aws_kms_key.ebs[0].id : null
}

output "efs_key_arn" {
  description = "ARN of the efs key"
  value       = var.efs ? aws_kms_key.efs[0].arn : null
}

output "efs_key_id" {
  description = "ID of the efs key"
  value       = var.efs ? aws_kms_key.efs[0].id : null
}

output "elasticache_key_arn" {
  description = "ARN of the ElastiCache key"
  value       = var.elasticache ? aws_kms_key.elasticache[0].arn : null
}

output "elasticache_key_id" {
  description = "ID of the ElastiCache key"
  value       = var.elasticache ? aws_kms_key.elasticache[0].id : null
}

output "rds_key_arn" {
  description = "ARN of the RDS key"
  value       = var.rds ? aws_kms_key.rds[0].arn : null
}

output "rds_key_id" {
  description = "ID of the RDS key"
  value       = var.rds ? aws_kms_key.rds[0].id : null
}

output "s3_key_arn" {
  description = "ARN of the S3 key"
  value       = var.s3 ? aws_kms_key.s3[0].arn : null
}

output "s3_key_id" {
  description = "ID of the S3 key"
  value       = var.s3 ? aws_kms_key.s3[0].id : null
}

output "secretsmanager_key_arn" {
  description = "ARN of the SecretsManager key"
  value       = var.secretsmanager ? aws_kms_key.secretsmanager[0].arn : null
}

output "secretsmanager_key_id" {
  description = "ID of the SecretsManager key"
  value       = var.secretsmanager ? aws_kms_key.secretsmanager[0].id : null
}

output "sns_key_arn" {
  description = "ARN of the SNS key"
  value       = var.sns ? aws_kms_key.sns[0].arn : null
}

output "sns_key_id" {
  description = "ID of the SNS key"
  value       = var.sns ? aws_kms_key.sns[0].id : null
}

output "sqs_key_arn" {
  description = "ARN of the sqs key"
  value       = var.sqs ? aws_kms_key.sqs[0].arn : null
}

output "sqs_key_id" {
  description = "ID of the sqs key"
  value       = var.sqs ? aws_kms_key.sqs[0].id : null
}

output "ssm_key_arn" {
  description = "ARN of the SSM key"
  value       = var.ssm ? aws_kms_key.ssm[0].arn : null
}

output "ssm_key_id" {
  description = "ID of the SSM key"
  value       = var.ssm ? aws_kms_key.ssm[0].id : null
}

output "workspaces_key_arn" {
  description = "ARN of the WorkSpaces key"
  value       = var.workspaces ? aws_kms_key.workspaces[0].arn : null
}

output "workspaces_key_id" {
  description = "ID of the WorkSpaces key"
  value       = var.workspaces ? aws_kms_key.workspaces[0].id : null
}
