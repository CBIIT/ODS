variable "identifier" {
  description = "Identifier for the RDS cluster"
  type        = string
}

variable "engine_version" {
  description = "PostgreSQL engine version"
  type        = string
  default     = "13.18"
}

variable "instance_class" {
  description = "Instance class to use"
  type        = string
  default     = "db.serverless"
}

variable "master_username" {
  description = "Master username for the database"
  type        = string
  default     = "postgres"
}

variable "master_password" {
  description = "Master password for the database"
  type        = string
  sensitive   = true
}

variable "vpc_id" {
  description = "VPC ID where the cluster will be created"
  type        = string
}

variable "subnet_ids" {
  description = "Subnet IDs where the cluster will be created"
  type        = list(string)
}

variable "deletion_protection" {
  description = "Enable or disable deletion protection"
  type        = bool
  default     = false
}

variable "backup_retention_period" {
  description = "Backup retention period in days"
  type        = number
  default     = 7
}

variable "preferred_backup_window" {
  description = "Preferred backup window"
  type        = string
  default     = "02:00-03:00"
}

variable "preferred_maintenance_window" {
  description = "Preferred maintenance window"
  type        = string
  default     = "sun:04:00-sun:05:00"
}

variable "tags" {
  description = "Tags to apply to the RDS cluster"
  type        = map(string)
  default     = {}
}

variable "multi_az" {
  description = "Enable multi-AZ deployment"
  type        = bool
  default     = true
}

variable "serverless_min_capacity" {
  description = "Minimum ACU for serverless"
  type        = number
  default     = 0.5
}

variable "serverless_max_capacity" {
  description = "Maximum ACU for serverless"
  type        = number
  default     = 16
}
