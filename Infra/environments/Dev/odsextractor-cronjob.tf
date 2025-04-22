resource "aws_iam_role" "odsextractor_ecs_events_task_role" {
  name = "odsextractor-ecsEventsTaskRole"
 
  assume_role_policy = <<EOF
{
   "Version":"2012-10-17",
   "Statement":[
      {
         "Effect":"Allow",
         "Principal":{
            "Service":[
               "ecs-tasks.amazonaws.com"
            ]
         },
         "Action":"sts:AssumeRole",
         "Condition":{
            "ArnLike":{
            "aws:SourceArn":"arn:aws:ecs:${var.odsextractor_region}:${local.myaccount}:*"
            },
            "StringEquals":{
               "aws:SourceAccount":"${local.myaccount}"
            }
         }
      }
   ]
}
EOF
}

 resource "aws_iam_role_policy_attachment" "odsextractor_ecs_events_task_role_policy_attachment" {
     role       = aws_iam_role.odsextractor_ecs_events_task_role.name
     policy_arn = "arn:aws:iam::aws:policy/service-role/AmazonEC2ContainerServiceEventsRole"
 }

########################### Define the ECS scheduled task for DATAPOINTROLESTATUS ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_DATAPOINTROLESTATUS" {
  name        = "odsextractor_DATAPOINTROLESTATUS"
  description = "odsextractor DATAPOINTROLESTATUS job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for DATAPOINTROLESTATUS
resource "aws_cloudwatch_event_target" "odsextractor_DATAPOINTROLESTATUS" {
  rule      = aws_cloudwatch_event_rule.odsextractor_DATAPOINTROLESTATUS.name
  target_id = "odsextractor_DATAPOINTROLESTATUS"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    propagate_tags    = "TASK_DEFINITION"
    tags              = {                            
      "Table"            = "DATAPOINTROLESTATUS"    
      "Env"              = "dev" 
      "ExtractorType"    = "ODSExtractor" 
    }
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=DATAPOINTROLESTATUS" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for DATAPOINTS ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_DATAPOINTS" {
  name        = "odsextractor_DATAPOINTS"
  description = "odsextractor DATAPOINTS job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for DATAPOINTS
resource "aws_cloudwatch_event_target" "odsextractor_DATAPOINTS" {
  rule      = aws_cloudwatch_event_rule.odsextractor_DATAPOINTS.name
  target_id = "odsextractor_DATAPOINTS"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor-high.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=DATAPOINTS" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for DATAPOINTREVIEWSTATUS ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_DATAPOINTREVIEWSTATUS" {
  name        = "odsextractor_DATAPOINTREVIEWSTATUS"
  description = "odsextractor DATAPOINTREVIEWSTATUS job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for DATAPOINTREVIEWSTATUS
resource "aws_cloudwatch_event_target" "odsextractor_DATAPOINTREVIEWSTATUS" {
  rule      = aws_cloudwatch_event_rule.odsextractor_DATAPOINTREVIEWSTATUS.name
  target_id = "odsextractor_DATAPOINTREVIEWSTATUS"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=DATAPOINTREVIEWSTATUS" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for REPORTINGRECORDSEXT2 ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_REPORTINGRECORDSEXT2" {
  name        = "odsextractor_REPORTINGRECORDSEXT2"
  description = "odsextractor REPORTINGRECORDSEXT2 job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for REPORTINGRECORDSEXT2
resource "aws_cloudwatch_event_target" "odsextractor_REPORTINGRECORDSEXT2" {
  rule      = aws_cloudwatch_event_rule.odsextractor_REPORTINGRECORDSEXT2.name
  target_id = "odsextractor_REPORTINGRECORDSEXT2"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor-high.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=REPORTINGRECORDSEXT2" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for DATADICTIONARYENTRIES ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_DATADICTIONARYENTRIES" {
  name        = "odsextractor_DATADICTIONARYENTRIES"
  description = "odsextractor DATADICTIONARYENTRIES job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for DATADICTIONARYENTRIES
resource "aws_cloudwatch_event_target" "odsextractor_DATADICTIONARYENTRIES" {
  rule      = aws_cloudwatch_event_rule.odsextractor_DATADICTIONARYENTRIES.name
  target_id = "odsextractor_DATADICTIONARYENTRIES"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=DATADICTIONARYENTRIES" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for RECORDS ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_RECORDS" {
  name        = "odsextractor_RECORDS"
  description = "odsextractor RECORDS job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for RECORDS
resource "aws_cloudwatch_event_target" "odsextractor_RECORDS" {
  rule      = aws_cloudwatch_event_rule.odsextractor_RECORDS.name
  target_id = "odsextractor_RECORDS"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=RECORDS" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"                     
             },
           ]
        })
}
########################### Define the ECS scheduled task for FIELDRESTRICTIONS ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_FIELDRESTRICTIONS" {
  name        = "odsextractor_FIELDRESTRICTIONS"
  description = "odsextractor FIELDRESTRICTIONS job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for FIELDRESTRICTIONS
resource "aws_cloudwatch_event_target" "odsextractor_FIELDRESTRICTIONS" {
  rule      = aws_cloudwatch_event_rule.odsextractor_FIELDRESTRICTIONS.name
  target_id = "odsextractor_FIELDRESTRICTIONS"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=FIELDRESTRICTIONS" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for REPORTINGLABDATAPOINTS ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_REPORTINGLABDATAPOINTS" {
  name        = "odsextractor_REPORTINGLABDATAPOINTS"
  description = "odsextractor REPORTINGLABDATAPOINTS job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for REPORTINGLABDATAPOINTS
resource "aws_cloudwatch_event_target" "odsextractor_REPORTINGLABDATAPOINTS" {
  rule      = aws_cloudwatch_event_rule.odsextractor_REPORTINGLABDATAPOINTS.name
  target_id = "odsextractor_REPORTINGLABDATAPOINTS"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=REPORTINGLABDATAPOINTS" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for REPORTINGRECORDS ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_REPORTINGRECORDS" {
  name        = "odsextractor_REPORTINGRECORDS"
  description = "odsextractor REPORTINGRECORDS job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for REPORTINGRECORDS
resource "aws_cloudwatch_event_target" "odsextractor_REPORTINGRECORDS" {
  rule      = aws_cloudwatch_event_rule.odsextractor_REPORTINGRECORDS.name
  target_id = "odsextractor_REPORTINGRECORDS"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor-high.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=REPORTINGRECORDS" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for FIELDS ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_FIELDS" {
  name        = "odsextractor_FIELDS"
  description = "odsextractor FIELDS job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for FIELDS
resource "aws_cloudwatch_event_target" "odsextractor_FIELDS" {
  rule      = aws_cloudwatch_event_rule.odsextractor_FIELDS.name
  target_id = "odsextractor_FIELDS"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=FIELDS" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for DATAPAGES ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_DATAPAGES" {
  name        = "odsextractor_DATAPAGES"
  description = "odsextractor DATAPAGES job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for DATAPAGES
resource "aws_cloudwatch_event_target" "odsextractor_DATAPAGES" {
  rule      = aws_cloudwatch_event_rule.odsextractor_DATAPAGES.name
  target_id = "odsextractor_DATAPAGES"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=DATAPAGES" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for VARIABLES ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_VARIABLES" {
  name        = "odsextractor_VARIABLES"
  description = "odsextractor VARIABLES job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for VARIABLES
resource "aws_cloudwatch_event_target" "odsextractor_VARIABLES" {
  rule      = aws_cloudwatch_event_rule.odsextractor_VARIABLES.name
  target_id = "odsextractor_VARIABLES"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=VARIABLES" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for FORMRESTRICTIONS ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_FORMRESTRICTIONS" {
  name        = "odsextractor_FORMRESTRICTIONS"
  description = "odsextractor FORMRESTRICTIONS job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for FORMRESTRICTIONS
resource "aws_cloudwatch_event_target" "odsextractor_FORMRESTRICTIONS" {
  rule      = aws_cloudwatch_event_rule.odsextractor_FORMRESTRICTIONS.name
  target_id = "odsextractor_FORMRESTRICTIONS"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=FORMRESTRICTIONS" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for INSTANCES ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_INSTANCES" {
  name        = "odsextractor_INSTANCES"
  description = "odsextractor INSTANCES job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for INSTANCES
resource "aws_cloudwatch_event_target" "odsextractor_INSTANCES" {
  rule      = aws_cloudwatch_event_rule.odsextractor_INSTANCES.name
  target_id = "odsextractor_INSTANCES"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=INSTANCES" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for SUBJECTMATRIX ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_SUBJECTMATRIX" {
  name        = "odsextractor_SUBJECTMATRIX"
  description = "odsextractor SUBJECTMATRIX job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for SUBJECTMATRIX
resource "aws_cloudwatch_event_target" "odsextractor_SUBJECTMATRIX" {
  rule      = aws_cloudwatch_event_rule.odsextractor_SUBJECTMATRIX.name
  target_id = "odsextractor_SUBJECTMATRIX"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=SUBJECTMATRIX" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for FOLDERFORMS ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_FOLDERFORMS" {
  name        = "odsextractor_FOLDERFORMS"
  description = "odsextractor FOLDERFORMS job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for FOLDERFORMS
resource "aws_cloudwatch_event_target" "odsextractor_FOLDERFORMS" {
  rule      = aws_cloudwatch_event_rule.odsextractor_FOLDERFORMS.name
  target_id = "odsextractor_FOLDERFORMS"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=FOLDERFORMS" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for DATADICTIONARIES ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_DATADICTIONARIES" {
  name        = "odsextractor_DATADICTIONARIES"
  description = "odsextractor DATADICTIONARIES job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for DATADICTIONARIES
resource "aws_cloudwatch_event_target" "odsextractor_DATADICTIONARIES" {
  rule      = aws_cloudwatch_event_rule.odsextractor_DATADICTIONARIES.name
  target_id = "odsextractor_DATADICTIONARIES"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=DATADICTIONARIES" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for SUBJECTROLESTATUS ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_SUBJECTROLESTATUS" {
  name        = "odsextractor_SUBJECTROLESTATUS"
  description = "odsextractor SUBJECTROLESTATUS job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for SUBJECTROLESTATUS
resource "aws_cloudwatch_event_target" "odsextractor_SUBJECTROLESTATUS" {
  rule      = aws_cloudwatch_event_rule.odsextractor_SUBJECTROLESTATUS.name
  target_id = "odsextractor_SUBJECTROLESTATUS"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=SUBJECTROLESTATUS" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for USERSTUDYSITES ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_USERSTUDYSITES" {
  name        = "odsextractor_USERSTUDYSITES"
  description = "odsextractor USERSTUDYSITES job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for USERSTUDYSITES
resource "aws_cloudwatch_event_target" "odsextractor_USERSTUDYSITES" {
  rule      = aws_cloudwatch_event_rule.odsextractor_USERSTUDYSITES.name
  target_id = "odsextractor_USERSTUDYSITES"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=USERSTUDYSITES" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for DERIVATIONSTEPS ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_DERIVATIONSTEPS" {
  name        = "odsextractor_DERIVATIONSTEPS"
  description = "odsextractor DERIVATIONSTEPS job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for DERIVATIONSTEPS
resource "aws_cloudwatch_event_target" "odsextractor_DERIVATIONSTEPS" {
  rule      = aws_cloudwatch_event_rule.odsextractor_DERIVATIONSTEPS.name
  target_id = "odsextractor_DERIVATIONSTEPS"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=DERIVATIONSTEPS" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for USEROBJECTROLE ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_USEROBJECTROLE" {
  name        = "odsextractor_USEROBJECTROLE"
  description = "odsextractor USEROBJECTROLE job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for USEROBJECTROLE
resource "aws_cloudwatch_event_target" "odsextractor_USEROBJECTROLE" {
  rule      = aws_cloudwatch_event_rule.odsextractor_USEROBJECTROLE.name
  target_id = "odsextractor_USEROBJECTROLE"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=USEROBJECTROLE" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for FORMS ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_FORMS" {
  name        = "odsextractor_FORMS"
  description = "odsextractor FORMS job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for FORMS
resource "aws_cloudwatch_event_target" "odsextractor_FORMS" {
  rule      = aws_cloudwatch_event_rule.odsextractor_FORMS.name
  target_id = "odsextractor_FORMS"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=FORMS" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for FOLDERS ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_FOLDERS" {
  name        = "odsextractor_FOLDERS"
  description = "odsextractor FOLDERS job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for FOLDERS
resource "aws_cloudwatch_event_target" "odsextractor_FOLDERS" {
  rule      = aws_cloudwatch_event_rule.odsextractor_FOLDERS.name
  target_id = "odsextractor_FOLDERS"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=FOLDERS" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for REPORTINGLABDPDELETES ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_REPORTINGLABDPDELETES" {
  name        = "odsextractor_REPORTINGLABDPDELETES"
  description = "odsextractor REPORTINGLABDPDELETES job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for REPORTINGLABDPDELETES
resource "aws_cloudwatch_event_target" "odsextractor_REPORTINGLABDPDELETES" {
  rule      = aws_cloudwatch_event_rule.odsextractor_REPORTINGLABDPDELETES.name
  target_id = "odsextractor_REPORTINGLABDPDELETES"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=REPORTINGLABDPDELETES" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for LOCALIZEDDATASTRINGS ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_LOCALIZEDDATASTRINGS" {
  name        = "odsextractor_LOCALIZEDDATASTRINGS"
  description = "odsextractor LOCALIZEDDATASTRINGS job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for LOCALIZEDDATASTRINGS
resource "aws_cloudwatch_event_target" "odsextractor_LOCALIZEDDATASTRINGS" {
  rule      = aws_cloudwatch_event_rule.odsextractor_LOCALIZEDDATASTRINGS.name
  target_id = "odsextractor_LOCALIZEDDATASTRINGS"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=LOCALIZEDDATASTRINGS" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for DERIVATIONS ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_DERIVATIONS" {
  name        = "odsextractor_DERIVATIONS"
  description = "odsextractor DERIVATIONS job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for DERIVATIONS
resource "aws_cloudwatch_event_target" "odsextractor_DERIVATIONS" {
  rule      = aws_cloudwatch_event_rule.odsextractor_DERIVATIONS.name
  target_id = "odsextractor_DERIVATIONS"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=DERIVATIONS" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for LOCALIZEDDATASTRINGPKS ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_LOCALIZEDDATASTRINGPKS" {
  name        = "odsextractor_LOCALIZEDDATASTRINGPKS"
  description = "odsextractor LOCALIZEDDATASTRINGPKS job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for LOCALIZEDDATASTRINGPKS
resource "aws_cloudwatch_event_target" "odsextractor_LOCALIZEDDATASTRINGPKS" {
  rule      = aws_cloudwatch_event_rule.odsextractor_LOCALIZEDDATASTRINGPKS.name
  target_id = "odsextractor_LOCALIZEDDATASTRINGPKS"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=LOCALIZEDDATASTRINGPKS" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for SUBJECTSTATUSHISTORY ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_SUBJECTSTATUSHISTORY" {
  name        = "odsextractor_SUBJECTSTATUSHISTORY"
  description = "odsextractor SUBJECTSTATUSHISTORY job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for SUBJECTSTATUSHISTORY
resource "aws_cloudwatch_event_target" "odsextractor_SUBJECTSTATUSHISTORY" {
  rule      = aws_cloudwatch_event_rule.odsextractor_SUBJECTSTATUSHISTORY.name
  target_id = "odsextractor_SUBJECTSTATUSHISTORY"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=SUBJECTSTATUSHISTORY" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for SUBJECTS ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_SUBJECTS" {
  name        = "odsextractor_SUBJECTS"
  description = "odsextractor SUBJECTS job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for SUBJECTS
resource "aws_cloudwatch_event_target" "odsextractor_SUBJECTS" {
  rule      = aws_cloudwatch_event_rule.odsextractor_SUBJECTS.name
  target_id = "odsextractor_SUBJECTS"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=SUBJECTS" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for LOCALIZEDSTRINGS ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_LOCALIZEDSTRINGS" {
  name        = "odsextractor_LOCALIZEDSTRINGS"
  description = "odsextractor LOCALIZEDSTRINGS job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for LOCALIZEDSTRINGS
resource "aws_cloudwatch_event_target" "odsextractor_LOCALIZEDSTRINGS" {
  rule      = aws_cloudwatch_event_rule.odsextractor_LOCALIZEDSTRINGS.name
  target_id = "odsextractor_LOCALIZEDSTRINGS"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=LOCALIZEDSTRINGS" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for USERS ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_USERS" {
  name        = "odsextractor_USERS"
  description = "odsextractor USERS job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for USERS
resource "aws_cloudwatch_event_target" "odsextractor_USERS" {
  rule      = aws_cloudwatch_event_rule.odsextractor_USERS.name
  target_id = "odsextractor_USERS"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=USERS" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for STUDYSITES ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_STUDYSITES" {
  name        = "odsextractor_STUDYSITES"
  description = "odsextractor STUDYSITES job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for STUDYSITES
resource "aws_cloudwatch_event_target" "odsextractor_STUDYSITES" {
  rule      = aws_cloudwatch_event_rule.odsextractor_STUDYSITES.name
  target_id = "odsextractor_STUDYSITES"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=STUDYSITES" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for EXTERNALUSERS ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_EXTERNALUSERS" {
  name        = "odsextractor_EXTERNALUSERS"
  description = "odsextractor EXTERNALUSERS job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for EXTERNALUSERS
resource "aws_cloudwatch_event_target" "odsextractor_EXTERNALUSERS" {
  rule      = aws_cloudwatch_event_rule.odsextractor_EXTERNALUSERS.name
  target_id = "odsextractor_EXTERNALUSERS"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=EXTERNALUSERS" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for LABASSIGNMENTS ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_LABASSIGNMENTS" {
  name        = "odsextractor_LABASSIGNMENTS"
  description = "odsextractor LABASSIGNMENTS job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for LABASSIGNMENTS
resource "aws_cloudwatch_event_target" "odsextractor_LABASSIGNMENTS" {
  rule      = aws_cloudwatch_event_rule.odsextractor_LABASSIGNMENTS.name
  target_id = "odsextractor_LABASSIGNMENTS"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=LABASSIGNMENTS" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for SITES ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_SITES" {
  name        = "odsextractor_SITES"
  description = "odsextractor SITES job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for SITES
resource "aws_cloudwatch_event_target" "odsextractor_SITES" {
  rule      = aws_cloudwatch_event_rule.odsextractor_SITES.name
  target_id = "odsextractor_SITES"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=SITES" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for STUDIES ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_STUDIES" {
  name        = "odsextractor_STUDIES"
  description = "odsextractor STUDIES job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for STUDIES
resource "aws_cloudwatch_event_target" "odsextractor_STUDIES" {
  rule      = aws_cloudwatch_event_rule.odsextractor_STUDIES.name
  target_id = "odsextractor_STUDIES"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=STUDIES" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for VARIABLECHANGEAUDITS ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_VARIABLECHANGEAUDITS" {
  name        = "odsextractor_VARIABLECHANGEAUDITS"
  description = "odsextractor VARIABLECHANGEAUDITS job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for VARIABLECHANGEAUDITS
resource "aws_cloudwatch_event_target" "odsextractor_VARIABLECHANGEAUDITS" {
  rule      = aws_cloudwatch_event_rule.odsextractor_VARIABLECHANGEAUDITS.name
  target_id = "odsextractor_VARIABLECHANGEAUDITS"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=VARIABLECHANGEAUDITS" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for LABS ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_LABS" {
  name        = "odsextractor_LABS"
  description = "odsextractor LABS job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for LABS
resource "aws_cloudwatch_event_target" "odsextractor_LABS" {
  rule      = aws_cloudwatch_event_rule.odsextractor_LABS.name
  target_id = "odsextractor_LABS"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=LABS" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for PROJECTS ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_PROJECTS" {
  name        = "odsextractor_PROJECTS"
  description = "odsextractor PROJECTS job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for PROJECTS
resource "aws_cloudwatch_event_target" "odsextractor_PROJECTS" {
  rule      = aws_cloudwatch_event_rule.odsextractor_PROJECTS.name
  target_id = "odsextractor_PROJECTS"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=PROJECTS" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for CONFIGURATION ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_CONFIGURATION" {
  name        = "odsextractor_CONFIGURATION"
  description = "odsextractor CONFIGURATION job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for CONFIGURATION
resource "aws_cloudwatch_event_target" "odsextractor_CONFIGURATION" {
  rule      = aws_cloudwatch_event_rule.odsextractor_CONFIGURATION.name
  target_id = "odsextractor_CONFIGURATION"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=CONFIGURATION" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for ROLESUBJECTSTATUSACCESS ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_ROLESUBJECTSTATUSACCESS" {
  name        = "odsextractor_ROLESUBJECTSTATUSACCESS"
  description = "odsextractor ROLESUBJECTSTATUSACCESS job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for ROLESUBJECTSTATUSACCESS
resource "aws_cloudwatch_event_target" "odsextractor_ROLESUBJECTSTATUSACCESS" {
  rule      = aws_cloudwatch_event_rule.odsextractor_ROLESUBJECTSTATUSACCESS.name
  target_id = "odsextractor_ROLESUBJECTSTATUSACCESS"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=ROLESUBJECTSTATUSACCESS" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for LABUNITDICTIONARYENTRIES ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_LABUNITDICTIONARYENTRIES" {
  name        = "odsextractor_LABUNITDICTIONARYENTRIES"
  description = "odsextractor LABUNITDICTIONARYENTRIES job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for LABUNITDICTIONARYENTRIES
resource "aws_cloudwatch_event_target" "odsextractor_LABUNITDICTIONARYENTRIES" {
  rule      = aws_cloudwatch_event_rule.odsextractor_LABUNITDICTIONARYENTRIES.name
  target_id = "odsextractor_LABUNITDICTIONARYENTRIES"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=LABUNITDICTIONARYENTRIES" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for LABUNITS ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_LABUNITS" {
  name        = "odsextractor_LABUNITS"
  description = "odsextractor LABUNITS job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for LABUNITS
resource "aws_cloudwatch_event_target" "odsextractor_LABUNITS" {
  rule      = aws_cloudwatch_event_rule.odsextractor_LABUNITS.name
  target_id = "odsextractor_LABUNITS"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=LABUNITS" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for ROLESALLMODULES ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_ROLESALLMODULES" {
  name        = "odsextractor_ROLESALLMODULES"
  description = "odsextractor ROLESALLMODULES job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for ROLESALLMODULES
resource "aws_cloudwatch_event_target" "odsextractor_ROLESALLMODULES" {
  rule      = aws_cloudwatch_event_rule.odsextractor_ROLESALLMODULES.name
  target_id = "odsextractor_ROLESALLMODULES"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=ROLESALLMODULES" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for LABUNITDICTIONARIES ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_LABUNITDICTIONARIES" {
  name        = "odsextractor_LABUNITDICTIONARIES"
  description = "odsextractor LABUNITDICTIONARIES job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for LABUNITDICTIONARIES
resource "aws_cloudwatch_event_target" "odsextractor_LABUNITDICTIONARIES" {
  rule      = aws_cloudwatch_event_rule.odsextractor_LABUNITDICTIONARIES.name
  target_id = "odsextractor_LABUNITDICTIONARIES"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=LABUNITDICTIONARIES" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for RANGETYPEVARIABLES ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_RANGETYPEVARIABLES" {
  name        = "odsextractor_RANGETYPEVARIABLES"
  description = "odsextractor RANGETYPEVARIABLES job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for RANGETYPEVARIABLES
resource "aws_cloudwatch_event_target" "odsextractor_RANGETYPEVARIABLES" {
  rule      = aws_cloudwatch_event_rule.odsextractor_RANGETYPEVARIABLES.name
  target_id = "odsextractor_RANGETYPEVARIABLES"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=RANGETYPEVARIABLES" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for SUBJECTSTATUS ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_SUBJECTSTATUS" {
  name        = "odsextractor_SUBJECTSTATUS"
  description = "odsextractor SUBJECTSTATUS job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for SUBJECTSTATUS
resource "aws_cloudwatch_event_target" "odsextractor_SUBJECTSTATUS" {
  rule      = aws_cloudwatch_event_rule.odsextractor_SUBJECTSTATUS.name
  target_id = "odsextractor_SUBJECTSTATUS"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=SUBJECTSTATUS" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for FIELDOIDDIRECTORY ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_FIELDOIDDIRECTORY" {
  name        = "odsextractor_FIELDOIDDIRECTORY"
  description = "odsextractor FIELDOIDDIRECTORY job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for FIELDOIDDIRECTORY
resource "aws_cloudwatch_event_target" "odsextractor_FIELDOIDDIRECTORY" {
  rule      = aws_cloudwatch_event_rule.odsextractor_FIELDOIDDIRECTORY.name
  target_id = "odsextractor_FIELDOIDDIRECTORY"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=FIELDOIDDIRECTORY" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for LABSTANDARDGROUPENTRIES ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_LABSTANDARDGROUPENTRIES" {
  name        = "odsextractor_LABSTANDARDGROUPENTRIES"
  description = "odsextractor LABSTANDARDGROUPENTRIES job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for LABSTANDARDGROUPENTRIES
resource "aws_cloudwatch_event_target" "odsextractor_LABSTANDARDGROUPENTRIES" {
  rule      = aws_cloudwatch_event_rule.odsextractor_LABSTANDARDGROUPENTRIES.name
  target_id = "odsextractor_LABSTANDARDGROUPENTRIES"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=LABSTANDARDGROUPENTRIES" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for LABSTANDARDGROUPS ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_LABSTANDARDGROUPS" {
  name        = "odsextractor_LABSTANDARDGROUPS"
  description = "odsextractor LABSTANDARDGROUPS job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for LABSTANDARDGROUPS
resource "aws_cloudwatch_event_target" "odsextractor_LABSTANDARDGROUPS" {
  rule      = aws_cloudwatch_event_rule.odsextractor_LABSTANDARDGROUPS.name
  target_id = "odsextractor_LABSTANDARDGROUPS"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=LABSTANDARDGROUPS" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for LOCALIZATIONCONTEXTS ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_LOCALIZATIONCONTEXTS" {
  name        = "odsextractor_LOCALIZATIONCONTEXTS"
  description = "odsextractor LOCALIZATIONCONTEXTS job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for LOCALIZATIONCONTEXTS
resource "aws_cloudwatch_event_target" "odsextractor_LOCALIZATIONCONTEXTS" {
  rule      = aws_cloudwatch_event_rule.odsextractor_LOCALIZATIONCONTEXTS.name
  target_id = "odsextractor_LOCALIZATIONCONTEXTS"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=LOCALIZATIONCONTEXTS" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for LOCALIZATIONS ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_LOCALIZATIONS" {
  name        = "odsextractor_LOCALIZATIONS"
  description = "odsextractor LOCALIZATIONS job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for LOCALIZATIONS
resource "aws_cloudwatch_event_target" "odsextractor_LOCALIZATIONS" {
  rule      = aws_cloudwatch_event_rule.odsextractor_LOCALIZATIONS.name
  target_id = "odsextractor_LOCALIZATIONS"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=LOCALIZATIONS" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for SITEGROUPS ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_SITEGROUPS" {
  name        = "odsextractor_SITEGROUPS"
  description = "odsextractor SITEGROUPS job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for SITEGROUPS
resource "aws_cloudwatch_event_target" "odsextractor_SITEGROUPS" {
  rule      = aws_cloudwatch_event_rule.odsextractor_SITEGROUPS.name
  target_id = "odsextractor_SITEGROUPS"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=SITEGROUPS" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for PROJECTSOURCESYSTEMR ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_PROJECTSOURCESYSTEMR" {
  name        = "odsextractor_PROJECTSOURCESYSTEMR"
  description = "odsextractor PROJECTSOURCESYSTEMR job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for PROJECTSOURCESYSTEMR
resource "aws_cloudwatch_event_target" "odsextractor_PROJECTSOURCESYSTEMR" {
  rule      = aws_cloudwatch_event_rule.odsextractor_PROJECTSOURCESYSTEMR.name
  target_id = "odsextractor_PROJECTSOURCESYSTEMR"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=PROJECTSOURCESYSTEMR" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for SUBJECTSTATUSCATEGORYR ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_SUBJECTSTATUSCATEGORYR" {
  name        = "odsextractor_SUBJECTSTATUSCATEGORYR"
  description = "odsextractor SUBJECTSTATUSCATEGORYR job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for SUBJECTSTATUSCATEGORYR
resource "aws_cloudwatch_event_target" "odsextractor_SUBJECTSTATUSCATEGORYR" {
  rule      = aws_cloudwatch_event_rule.odsextractor_SUBJECTSTATUSCATEGORYR.name
  target_id = "odsextractor_SUBJECTSTATUSCATEGORYR"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=SUBJECTSTATUSCATEGORYR" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for LABUNITCONVERSIONS ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_LABUNITCONVERSIONS" {
  name        = "odsextractor_LABUNITCONVERSIONS"
  description = "odsextractor LABUNITCONVERSIONS job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for LABUNITCONVERSIONS
resource "aws_cloudwatch_event_target" "odsextractor_LABUNITCONVERSIONS" {
  rule      = aws_cloudwatch_event_rule.odsextractor_LABUNITCONVERSIONS.name
  target_id = "odsextractor_LABUNITCONVERSIONS"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=LABUNITCONVERSIONS" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for LABUPDATEQUEUE ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_LABUPDATEQUEUE" {
  name        = "odsextractor_LABUPDATEQUEUE"
  description = "odsextractor LABUPDATEQUEUE job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for LABUPDATEQUEUE
resource "aws_cloudwatch_event_target" "odsextractor_LABUPDATEQUEUE" {
  rule      = aws_cloudwatch_event_rule.odsextractor_LABUPDATEQUEUE.name
  target_id = "odsextractor_LABUPDATEQUEUE"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=LABUPDATEQUEUE" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for UPLOADDATAPOINTS ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_UPLOADDATAPOINTS" {
  name        = "odsextractor_UPLOADDATAPOINTS"
  description = "odsextractor UPLOADDATAPOINTS job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for UPLOADDATAPOINTS
resource "aws_cloudwatch_event_target" "odsextractor_UPLOADDATAPOINTS" {
  rule      = aws_cloudwatch_event_rule.odsextractor_UPLOADDATAPOINTS.name
  target_id = "odsextractor_UPLOADDATAPOINTS"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=UPLOADDATAPOINTS" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for UNITDICTIONARIES ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_UNITDICTIONARIES" {
  name        = "odsextractor_UNITDICTIONARIES"
  description = "odsextractor UNITDICTIONARIES job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for UNITDICTIONARIES
resource "aws_cloudwatch_event_target" "odsextractor_UNITDICTIONARIES" {
  rule      = aws_cloudwatch_event_rule.odsextractor_UNITDICTIONARIES.name
  target_id = "odsextractor_UNITDICTIONARIES"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=UNITDICTIONARIES" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for UNITDICTIONARYENTRIES ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_UNITDICTIONARYENTRIES" {
  name        = "odsextractor_UNITDICTIONARYENTRIES"
  description = "odsextractor UNITDICTIONARYENTRIES job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for UNITDICTIONARYENTRIES
resource "aws_cloudwatch_event_target" "odsextractor_UNITDICTIONARYENTRIES" {
  rule      = aws_cloudwatch_event_rule.odsextractor_UNITDICTIONARYENTRIES.name
  target_id = "odsextractor_UNITDICTIONARYENTRIES"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=UNITDICTIONARYENTRIES" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for CLINICALSIGNIFICANCE ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_CLINICALSIGNIFICANCE" {
  name        = "odsextractor_CLINICALSIGNIFICANCE"
  description = "odsextractor CLINICALSIGNIFICANCE job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for CLINICALSIGNIFICANCE
resource "aws_cloudwatch_event_target" "odsextractor_CLINICALSIGNIFICANCE" {
  rule      = aws_cloudwatch_event_rule.odsextractor_CLINICALSIGNIFICANCE.name
  target_id = "odsextractor_CLINICALSIGNIFICANCE"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=CLINICALSIGNIFICANCE" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}
########################### Define the ECS scheduled task for CLINICALSIGNIFICANCECODES ###########################
resource "aws_cloudwatch_event_rule" "odsextractor_CLINICALSIGNIFICANCECODES" {
  name        = "odsextractor_CLINICALSIGNIFICANCECODES"
  description = "odsextractor CLINICALSIGNIFICANCECODES job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for CLINICALSIGNIFICANCECODES
resource "aws_cloudwatch_event_target" "odsextractor_CLINICALSIGNIFICANCECODES" {
  rule      = aws_cloudwatch_event_rule.odsextractor_CLINICALSIGNIFICANCECODES.name
  target_id = "odsextractor_CLINICALSIGNIFICANCECODES"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsextractor.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsextractor.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsextractor_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSExtractor_Incremental", "--tableName=CLINICALSIGNIFICANCECODES" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsextractor_container_name}"
             },
            ]
        })
}