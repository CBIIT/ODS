variable "s3_block_all_public_access" {
  description = "Block S3 public access to the logging bucket"
  default     = true
  type        = bool
}

variable "bucket_name" {
  description = "S3 Config bucket name.  Recommend config-ACCOUNT_ID."
  type        = string
}

variable "delivery_frequency" {
  description = "Config snapshot delivery frequency."
  default     = "One_Hour"
  type        = string
}

variable "configuration_recorder_name" {
  description = "Name for the configuration recorder.  Only visible via API."
  default     = "config-recorder"
  type        = string
}

variable "delivery_channel_name" {
  description = "Name for the delivery channel.  Only visible via API."
  default     = "config-delivery"
  type        = string
}

variable "tags" {
  description = "Tags to apply to all stack resources."
  default     = {}
  type        = map(any)
}

variable "topic_name" {
  description = "SNS topic name."
  default     = "config-notifications"
}
