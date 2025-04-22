############################
##### App Variables #####
############################

variable "odsextractor_env" {
    description = "ODS Extractor Environment"
    type        = string
    default     = "dev-nci"
}

variable "odsextractor_s3_bucket" {
    description = "ODS Extractor S3 Bucket"
    type        = string
    default     = "ods-table-data"
}

variable "odsextractor_region" {
    description = "ODS Extractor Region"
    type        = string
    default     = "us-east-1"
}

variable "odsextractor_cluster_name" {
    description = "ODS Extractor Cluster Name"
    type        = string    
    default    = "theradex-dev-nci-cluster-odsextractor"
}

variable "odsextractor_taskdefinition_name" {
    description = "ODS Extractor Task Definition Name"
    type        = string
    default     = "TheradexODSExtractor"
}

variable "odsextractor_taskdefinition_name_high" {
    description = "ODS Extractor Task Definition Name High"
    type        = string
    default     = "TheradexODSExtractor-High"
}

variable "odsextractor_container_name" {
    description = "ODS Extractor ECS Container Name"
    type        = string
    default     = "Dev-TheradexODSExtractor"
}

variable "odsextractor_taskdefinition_cloudwatch_group_name" {
    description = "ODS Extractor Task Definition Cloud Watch Group Name"
    type        = string
    default     = "dev-nci-odsextractor"
}

variable "odsextractor_logs_retention_in_days" {
  type        = number
  default     = 90
  description = "Specifies the number of days you want to retain log events"
}