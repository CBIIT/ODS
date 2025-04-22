variable "backup" {
  description = "Enable customer managed KMS key for use with AWS Backup"
  default     = false
  type        = bool
}

variable "dynamodb" {
  description = "Enable customer managed KMS key for use with DynamoDB Tables"
  default     = false
  type        = bool
}

variable "ebs" {
  description = "Enable customer managed KMS key for use with EBS volumes"
  default     = true
  type        = bool
}

variable "efs" {
  description = "Enable customer managed KMS key for use with EFS"
  default     = false
  type        = bool
}

variable "elasticache" {
  description = "Enable customer managed KMS key for ElastiCache"
  default     = false
  type        = bool
}

variable "rds" {
  description = "Enable customer managed KMS key for RDS"
  default     = false
  type        = bool
}

variable "s3" {
  description = "Enable customer managed KMS key for use with s3 buckets"
  default     = false
  type        = bool
}

variable "secretsmanager" {
  description = "Enable customer managed KMS key for SecretsManager"
  default     = false
  type        = bool
}

variable "sqs" {
  description = "Enable customer managed KMS key for SQS"
  default     = false
  type        = bool
}

variable "ssm" {
  description = "Enable customer managed KMS key for SSM"
  default     = false
  type        = bool
}

variable "sns" {
  description = "Enable customer managed KMS key for SNS"
  default     = false
  type        = bool
}

variable "workspaces" {
  description = "Enable customer managed KMS key for WorkSpaces"
  default     = false
  type        = bool
}

variable "ebs_encryption_by_default" {
  description = "Enable EBS volume encryption by default using customer managed KMS key"
  default     = false
  type        = bool
}

variable "kms_key_rotation" {
  description = "AWS managed rotation of KMS key.  Occurs automatically each year."
  default     = true
  type        = bool
}

variable "kms_key_deletion_window_in_days" {
  description = "Number of days before a key is removed after being marked for deletion.  7-30 days."
  default     = 30
  type        = number
}


variable "service_linked_role_spot_arn" {
  description = "Spot service linked role ARN.  If supplied, proper access to the EBS key will be granted."
  default     = ""
  type        = string
}

variable "service_role_batch_arn" {
  description = "Batch service role ARN.  If supplied, proper access to the EBS key will be granted."
  default     = ""
  type        = string
}

variable "service_role_spot_fleet_arn" {
  description = "Spot Fleet service role ARN.  If supplied, proper access to the EBS key will be granted."
  default     = ""
  type        = string
}

variable "tags" {
  description = "Tags to apply to all module resources."
  default     = {}
  type        = map(any)
}
