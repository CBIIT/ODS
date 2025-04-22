variable "corporate_cidr" {
  description = "CIDR block for on-premise connectivity"
  default     = ""
  type        = string
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
