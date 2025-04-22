variable "bucket_name" {
  description = "S3 CloudTrail bucket name.  Recommend cloudtrail-ACCOUNT_ID."
  type        = string
}

variable "enable_log_file_validation" {
  description = "Log file integrity validation."
  default     = true
  type        = bool
}

variable "include_global_service_events" {
  description = "Publish events from global services (IAM, etc) to this trail."
  default     = true
  type        = bool
}

variable "is_multi_region_trail" {
  description = "Collect events from all regions."
  default     = true
  type        = bool
}

variable "is_organization_trail" {
  description = "Creates an organization wide CloudTrail."
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

variable "name" {
  description = "Trail name.  Recommend cloudtrail-ACCOUNT_ID."
  type        = string
}

variable "retention_in_days" {
  description = "CloudWatch logs retention in days."
  default     = 180
  type        = number
}

variable "tags" {
  description = "Tags to apply to all stack resources."
  default     = {}
  type        = map(any)
}

variable "deny_delete" {
  description = "Deny S3:Delete on the CloudTrail bucket."
  default     = false
  type        = bool
}

variable "event_source_buckets" {
  description = "List of bucket names to monitor data events via CloudTrail. A single index with value '*' will enable for all buckets"
  default     = []
  type        = list(string)
}
