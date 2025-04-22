variable "auto_accept_shared_attachments" {
  description = "Auto accept shared attachments.  E.g. sub account VPC association"
  default     = "enable"
  type        = string
}

variable "allow_external_principals" {
  description = "Allow principals outside your AWS Organization"
  default     = false
  type        = bool
}

variable "name" {
  description = "Transit Gateway name.  Appended to related resources and tags"
  type        = string
}

variable "target_share_accounts" {
  description = "List of AWS accounts to share the transit gateway with."
  default     = []
  type        = list
}

variable "tags" {
  description = "Tags to apply to all module resources"
  default     = {}
  type        = map
}
