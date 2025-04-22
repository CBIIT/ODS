###############################
##### RWSSettings  #####
###############################

resource "aws_ssm_parameter" "theradex_app_odsmanager_rwssettings_rwsserver" {
  name          = "/${var.odsmanager_env}/app/odsmanager/RWSSettings/RWSServer"
  description   = "Theradex APP ODS Extractor RWSSettings RWSServer"
  type          = "String"
  value         = "https://theradex.mdsol.com"
}

resource "aws_ssm_parameter" "theradex_app_odsmanager_rwssettings_rwsusername" {
  name          = "/${var.odsmanager_env}/app/odsmanager/RWSSettings/RWSUserName"
  description   = "Theradex APP ODS Extractor RWSSettings RWSUserName"
  type          = "String"
  value         = "SPEC_TRACK_RWS"
}

resource "aws_ssm_parameter" "theradex_app_odsmanager_rwssettings_rwspassword" {
  name          = "/${var.odsmanager_env}/app/odsmanager/RWSSettings/RWSPassword"
  description   = "Theradex APP ODS Extractor RWSSettings RWSPassword"
  type          = "String"
  value         = "Password@01"
}

resource "aws_ssm_parameter" "theradex_app_odsmanager_rwssettings_timeoutinsecs" {
  name          = "/${var.odsmanager_env}/app/odsmanager/RWSSettings/TimeoutInSecs"
  description   = "Theradex APP ODS Extractor RWSSettings TimeoutInSecs"
  type          = "String"
  value         = "3600"
}

###############################
##### EmailSettings #####
###############################

resource "aws_ssm_parameter" "theradex_app_odsmanager_emailsettings_fromaddress" {
  name          = "/${var.odsmanager_env}/app/odsmanager/EmailSettings/FromAddress"
  description   = "Theradex APP ODS Extractor EmailSettings FromAddress"
  type          = "String"
  value         = "dev-noreply@theradex.com"
}

resource "aws_ssm_parameter" "theradex_app_odsmanager_emailsettings_toaddress" {
  name          = "/${var.odsmanager_env}/app/odsmanager/EmailSettings/ToAddress"
  description   = "Theradex APP ODS Extractor EmailSettings ToAddress"
  type          = "String"
  value         = "mrathi@theradex.com,nsarakhawas@theradex.com,uvarada@theradex.com"
}

###############################
##### AppSettings #####
###############################

resource "aws_ssm_parameter" "theradex_app_odsmanager_appsettings_archivebucket" {
  name          = "/${var.odsmanager_env}/app/odsmanager/AppSettings/ArchiveBucket"
  description   = "Theradex APP ODS Extractor AppSettings Archive Bucket"
  type          = "String"
  value         = var.odsmanager_s3_bucket
}

###############################
##### ODSSettings  #####
###############################

resource "aws_ssm_parameter" "theradex_app_odsmanager_odssettings_host" {
  name          = "/${var.odsmanager_env}/app/odsmanager/ODSSettings/Host"
  description   = "Theradex APP ODS Extractor ODSSettings Host"
  type          = "String"
  value         = "ods.cluster-cgntten8b01g.us-west-1.rds.amazonaws.com"
}

resource "aws_ssm_parameter" "theradex_app_odsmanager_odssettings_port" {
  name          = "/${var.odsmanager_env}/app/odsmanager/ODSSettings/Port"
  description   = "Theradex APP ODS Extractor ODSSettings Port"
  type          = "String"
  value         = "5432"
}

resource "aws_ssm_parameter" "theradex_app_odsmanager_odssettings_username" {
  name          = "/${var.odsmanager_env}/app/odsmanager/ODSSettings/Username"
  description   = "Theradex APP ODS Extractor ODSSettings Username"
  type          = "String"
  value         = "postgres"
}

resource "aws_ssm_parameter" "theradex_app_odsmanager_odssettings_password" {
  name          = "/${var.odsmanager_env}/app/odsmanager/ODSSettings/Password"
  description   = "Theradex APP ODS Extractor ODSSettings Password"
  type          = "String"
  value         = "Password0001"
}

resource "aws_ssm_parameter" "theradex_app_odsmanager_odssettings_timeoutinsecs" {
  name          = "/${var.odsmanager_env}/app/odsmanager/ODSSettings/TimeoutInSecs"
  description   = "Theradex APP ODS Extractor ODSSettings TimeoutInSecs"
  type          = "String"
  value         = "60"
}

resource "aws_ssm_parameter" "theradex_app_odsmanager_odssettings_database" {
  name          = "/${var.odsmanager_env}/app/odsmanager/ODSSettings/Database"
  description   = "Theradex APP ODS Extractor ODSSettings Database"
  type          = "String"
  value         = "ods"
}