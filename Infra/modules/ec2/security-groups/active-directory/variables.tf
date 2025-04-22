variable "corporate_domain_controller_cidrs" {
  description = "List of CIDR blocks for on-premise Domain Controllers"
  default     = []
  type        = list
}

variable "tags" {
  description = "Tags to apply to all module resources."
  default     = {}
  type        = map
}

variable "vpc_cidr" {
  description = "VPC CIDR"
  type        = string
}

variable "vpc_id" {
  description = "VPC ID"
  type        = string
}
