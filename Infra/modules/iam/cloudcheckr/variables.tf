variable "external_id" {
  description = "Role external ID.  Provided after CloudCheckr account creation. Example: CC-XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"
  type        = string
}

variable "s3_cloudtrail_bucket" {
  default     = ""
  description = "If empty, cloudtrail-<ACCOUNTID> will be substituted."
  type        = string
}

variable "s3_cost_and_usage_bucket" {
  default     = ""
  description = "If this account is your Organization payer account, enter the name of the CUR bucket."
  type        = string
}
