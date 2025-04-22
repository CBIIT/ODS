resource "aws_iam_role" "odsmanager_ecs_events_task_role" {
  name = "odsmanager-ecsEventsTaskRole"
 
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
            "aws:SourceArn":"arn:aws:ecs:${var.odsmanager_region}:${local.myaccount}:*"
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

 resource "aws_iam_role_policy_attachment" "odsmanager_ecs_events_task_role_policy_attachment" {
     role       = aws_iam_role.odsmanager_ecs_events_task_role.name
     policy_arn = "arn:aws:iam::aws:policy/service-role/AmazonEC2ContainerServiceEventsRole"
 }

########################### Define the ECS scheduled task for ALL ###########################
resource "aws_cloudwatch_event_rule" "odsmanager_ALL" {
  name        = "odsmanager_ALL"
  description = "odsmanager ALL job"
state = "ENABLED"
  schedule_expression = "rate(5 minutes)"
}
# Grant permissions to CloudWatch Events to run the ECS task for ALL
resource "aws_cloudwatch_event_target" "odsmanager_ALL" {
  rule      = aws_cloudwatch_event_rule.odsmanager_ALL.name
  target_id = "odsmanager_ALL"
  role_arn = "arn:aws:iam::${local.myaccount}:role/ecsEventsRole"
  arn      = aws_ecs_cluster.theradex_dev_nci_cluster_odsmanager.id
  count = 1
  ecs_target {
    task_count        = 1
    task_definition_arn = aws_ecs_task_definition.odsmanager.arn
    launch_type       = "FARGATE"
    platform_version  = "LATEST"
    propagate_tags    = "TASK_DEFINITION"
    tags              = {                            
      "Table"            = "NONE"    
      "Env"              = "dev" 
      "ExtractorType"    = "odsmanager" 
    }
    network_configuration {
      assign_public_ip = false
      security_groups  = [ aws_security_group.theradex_app_odsmanager_sg.id ]
      subnets          = local.private_subnet_ids
    }
  }
   input  = jsonencode(
        {
            containerOverrides = [
             {
                command          = ["--managerType=ODSManager_Incremental", "--tableName=NONE" ,"--env=dev" , "--raveDataUrl=/RaveWebServices/datasets/ThxExtractsByUpdatedDate.json", "--noofRecords=1"]
                environmentFiles = []
                name             = "${var.odsmanager_container_name}"
             },
            ]
        })
}