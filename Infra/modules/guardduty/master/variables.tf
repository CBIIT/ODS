variable "enable" {
  description = "Enable GuardDuty"
  type        = bool
  default     = true
}

variable "invite_message" {
  description = "Message to send in invite"
  type        = string
  default     = "Please accept this GuardDuty invitation."
}

variable "invite_accounts" {
  description = "Map of Account IDs and emails to invite to this master account"
  type        = map
  default     = {}
}

variable "finding_publishing_frequency" {
  description = "Notification frequency - FIFTEEN_MINUTES, ONE_HOUR, or SIX_HOURS"
  type        = string
  default     = "SIX_HOURS"
}
