variable "environment" {
  description = "Environment name"
  type        = string
  default     = "dev-nci"
}

variable "aws_region" {
  description = "AWS Region for the database"
  type        = string
  default     = "us-west-1"
}

variable "db_name" {
  description = "Database name"
  type        = string
  default     = "ods"
}

variable "db_username" {
  description = "Master username for the database"
  type        = string
  default     = "postgres"
}

variable "db_port" {
  description = "Database port"
  type        = number
  default     = 5432
}

variable "db_engine_version" {
  description = "Aurora PostgreSQL engine version"
  type        = string
  default     = "13.18"
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

variable "db_serverless_min_capacity" {
  description = "Minimum ACU for serverless"
  type        = number
  default     = 0.5
}

variable "db_serverless_max_capacity" {
  description = "Maximum ACU for serverless"
  type        = number
  default     = 16
}

variable "db_deletion_protection" {
  description = "Enable deletion protection"
  type        = bool
  default     = false
}
variable "nci-ods-s3-bucket-name" {
  description = "S3 bucket name for ODS"
  type        = string
  default     = "ods-table-data"
  
}

variable "tags" {
  description = "Default tags for all resources"
  type        = map(string)
  default     = {
    Environment = "dev-nci"
    Application = "ODS"
    Terraform   = "true"
  }
}
