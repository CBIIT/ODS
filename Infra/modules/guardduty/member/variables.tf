variable "enable" {
  description = "Enable GuardDuty"
  type        = bool
  default     = true
}

variable "master_aws_account_id" {
  description = "Account ID of master account"
  type        = string
}
