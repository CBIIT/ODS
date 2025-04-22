# Reference to existing VPC and subnets
# These would come from your existing infrastructure
data "aws_vpc" "dev_nci_vpc" {
  id = module.dev_noncomm_vpc.id 
}

data "aws_subnets" "database_subnets" {
  filter {
    name   = "vpc-id"
    values = [data.aws_vpc.dev_nci_vpc.id]
  }
  filter {
    name   = "tag:Name"
    values = ["*database*"]  # Adjust this filter based on how your subnets are tagged
  }
}

# KMS key for encryption
resource "aws_kms_key" "ods_encryption_key" {
  description             = "KMS key for ODS database encryption"
  deletion_window_in_days = 7
  enable_key_rotation     = true
}

resource "aws_kms_alias" "ods_encryption_key_alias" {
  name          = "alias/ods-encryption-key"
  target_key_id = aws_kms_key.ods_encryption_key.key_id
}

# Create a random password for the database
resource "random_password" "master_password" {
  length           = 16
  special          = true
  override_special = "!#$%&*()-_=+[]{}<>:?"
}

# Store the password in AWS Secrets Manager
resource "aws_secretsmanager_secret" "ods_db_credentials" {
  name                    = "/dev-nci/ods-database-credentials"
  recovery_window_in_days = 0  # Set to 0 for testing, use a higher value in production
}

resource "aws_secretsmanager_secret_version" "ods_db_credentials" {
  secret_id     = aws_secretsmanager_secret.ods_db_credentials.id
  secret_string = jsonencode({
    username = "postgres"
    password = random_password.master_password.result
  })
}

# Use the Aurora PostgreSQL module
module "ods_database" {
  source = "../../../../modules/rds-aurora-postgres"

  identifier             = var.db_name
  engine_version         = var.db_engine_version
  vpc_id                 = data.aws_vpc.dev_nci_vpc.id
  subnet_ids             = data.aws_subnets.database_subnets.ids
  master_username        = var.db_username
  master_password        = random_password.master_password.result
  deletion_protection    = var.db_deletion_protection
  multi_az               = true   
  serverless_min_capacity = var.db_serverless_min_capacity
  serverless_max_capacity = var.db_serverless_max_capacity
  backup_retention_period = var.backup_retention_period
  preferred_backup_window = var.preferred_backup_window
  preferred_maintenance_window = var.preferred_maintenance_window
  
  tags = var.tags
}

# Update the SSM parameters to point to the new database
# resource "aws_ssm_parameter" "theradex_app_odsextractor_odssettings_host" {
#   name          = "/dev-nci/app/odsextractor/ODSSettings/Host"
#   description   = "Theradex APP ODS Extractor ODSSettings Host"
#   type          = "String"
#   value         = module.ods_database.cluster_endpoint
# }

# resource "aws_ssm_parameter" "theradex_app_odsextractor_odssettings_password" {
#   name          = "/dev-nci/app/odsextractor/ODSSettings/Password"
#   description   = "Theradex APP ODS Extractor ODSSettings Password"
#   type          = "String"
#   value         = random_password.master_password.result
# }

# resource "aws_ssm_parameter" "theradex_app_odsmanager_odssettings_host" {
#   name          = "/dev-nci/app/odsmanager/ODSSettings/Host"
#   description   = "Theradex APP ODS Manager ODSSettings Host"
#   type          = "String"
#   value         = module.ods_database.cluster_endpoint
# }

# resource "aws_ssm_parameter" "theradex_app_odsmanager_odssettings_password" {
#   name          = "/dev-nci/app/odsmanager/ODSSettings/Password"
#   description   = "Theradex APP ODS Manager ODSSettings Password"
#   type          = "String"
#   value         = random_password.master_password.result
# }

# Output the database endpoints
output "ods_writer_endpoint" {
  description = "The endpoint for the writer instance"
  value       = module.ods_database.cluster_endpoint
}

output "ods_reader_endpoint" {
  description = "The endpoint for reader instances"
  value       = module.ods_database.cluster_reader_endpoint
}

output "ods_cluster_id" {
  description = "The cluster identifier"
  value       = module.ods_database.cluster_id
}