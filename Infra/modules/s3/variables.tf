variable "access_log_configuration" {
  description = "Access Logging Configuration"
  default     = []
  type        = list(map(string))
}

variable "acl" {
  description = "ACL"
  default     = "private"
  type        = string
}

variable "block_all_public_access" {
  description = "Block all public access"
  default     = true
  type        = bool
}

variable "kms_key_arn" {
  description = "KMS Key ARN for S3 encryption.  If empty, SSE/AES256 will be used."
  default     = ""
  type        = string
}

variable "lifecycle_rule" {
  description = "List of maps containing lifecycle rules"
  default     = []
  type        = list
}

variable "name" {
  description = "Bucket name"
  type        = string
}

variable "policy" {
  description = "Bucket policy"
  default     = ""
  type        = string
}

variable "tags" {
  description = "Tags to apply to all module resources."
  default     = {}
  type        = map
}

variable "versioning" {
  description = "Enable versioning"
  default     = "Disabled"
  type        = string
}
