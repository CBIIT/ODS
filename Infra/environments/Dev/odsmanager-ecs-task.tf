#Here lives the definition for the odsmanager task & service definition

resource "aws_iam_policy" "odsmanager_ecs_ssmpolicy" {
  name  = "odsmanager-ecs-ssmpolicy"

  policy = jsonencode({
            "Version": "2012-10-17",
            "Statement": [
                {
                    "Effect": "Allow",
                    "Action": [
                        "ssm:GetParametersByPath"
                    ],
                    "Resource": [
                        "*"
                    ]
                }
            ]
        })
}

resource "aws_iam_policy" "odsmanager_ecs_s3policy" {
  name  = "odsmanager-ecs-s3policy"

  policy = jsonencode({
                "Version": "2012-10-17",
                "Statement": [
                    {
                        "Sid": "odsmanagerAllowS3Access",
                        "Effect": "Allow",
                        "Action": [
                            "s3:PutObject",
                            "s3:GetObject",
                            "s3:AbortMultipartUpload",
                            "s3:ListBucket",
                            "s3:DeleteObject",
                            "s3:GetObjectVersion",
                            "s3:ListMultipartUploadParts",
                            "kms:GenerateDataKey"
                        ],
                        "Resource": [
                            "arn:aws:s3:::${var.odsmanager_s3_bucket}/*",
                            "arn:aws:s3:::${var.odsmanager_s3_bucket}"
                        ]
                    }
                ]
            })
}

resource "aws_iam_policy" "odsmanager_ecs_sespolicy" {
  name  = "odsmanager-ecs-sespolicy"

  policy = jsonencode({
                "Version": "2012-10-17",
                "Statement": [
                    {
                        "Effect": "Allow",
                        "Action": [
                            "ses:SendEmail",
                            "ses:SendRawEmail"
                        ],
                        "Resource": "*"
                    }
                ]
            })
}

resource "aws_iam_policy" "odsmanager_ecs_rdspolicy" {
  name  = "odsmanager_ecs_rdspolicy"

  policy = jsonencode({
                "Version": "2012-10-17",
                "Statement": [
                    {
                        "Effect": "Allow",
                        "Action": [
                            "rds-db:connect"
                        ],                        
                        "Resource":[                            
                            "arn:aws:rds-db:us-west-1:993530973844:dbuser:*/*"
                        ]
                    }
                ]
            })
}

resource "aws_iam_role" "odsmanager_ecs_task_role" {
  name = "odsmanager-ecsTaskRole"
 
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

resource "aws_iam_role" "odsmanager_ecs_task_execution_role" {
  name = "odsmanager-ecsTaskExecutionRole"
 
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
 
resource "aws_iam_role_policy_attachment" "odsmanager_ecs_task_execution_role_policy_attachment" {
  role = aws_iam_role.odsmanager_ecs_task_execution_role.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy"
}

resource "aws_iam_role_policy_attachment" "odsmanager_ecs_task_role_policy_attachment1" {
     role       = aws_iam_role.odsmanager_ecs_task_role.name
     policy_arn = aws_iam_policy.odsmanager_ecs_ssmpolicy.arn
 }

 resource "aws_iam_role_policy_attachment" "odsmanager_ecs_task_role_policy_attachment2" {
     role       = aws_iam_role.odsmanager_ecs_task_role.name
     policy_arn = aws_iam_policy.odsmanager_ecs_s3policy.arn
 }

resource "aws_iam_role_policy_attachment" "odsmanager_ecs_task_role_policy_attachment3" {
     role       = aws_iam_role.odsmanager_ecs_task_role.name
     policy_arn = aws_iam_policy.odsmanager_ecs_sespolicy.arn
 }

 resource "aws_iam_role_policy_attachment" "odsmanager_ecs_task_role_policy_attachment4" {
     role       = aws_iam_role.odsmanager_ecs_task_role.name
     policy_arn = aws_iam_policy.odsmanager_ecs_rdspolicy.arn
 }

 resource "aws_iam_role_policy_attachment" "odsmanager_ecs_task_role_policy_attachment5" {
     role       = aws_iam_role.odsmanager_ecs_task_role.name
     policy_arn = "arn:aws:iam::aws:policy/service-role/AmazonEC2ContainerServiceEventsRole"
 }

resource "aws_ecs_task_definition" "odsmanager" {
  family = var.odsmanager_taskdefinition_name
  network_mode = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  cpu = 1024
  memory = 2048 
  execution_role_arn = aws_iam_role.odsmanager_ecs_task_execution_role.arn
  task_role_arn = aws_iam_role.odsmanager_ecs_task_role.arn
  container_definitions = jsonencode([{
    name = "${var.odsmanager_container_name}"
    image = "${aws_ecr_repository.theradex_odsmanager_dev.repository_url}:dev-latest"
    essential = true
    environment = [
      { 
        name = "ASPNETCORE_ENVIRONMENT"
        value = "Dev Non-Commercial" 
      },
      {
          "name": "ODSEnvironment",
          "value": var.odsmanager_env
      }
    ]
    portMappings = [
      {
        protocol = "tcp"
        containerPort = 80
        hostPort = 80
      },
      {
        protocol = "tcp"
        containerPort = 443
        hostPort = 443
      },
    ]
    logConfiguration: {
        logDriver: "awslogs",
        options: {
          awslogs-group: "/ecs/task/${var.odsmanager_taskdefinition_cloudwatch_group_name}",
          awslogs-region: var.odsmanager_region,         
          awslogs-stream-prefix: "ecs"
        }
      }
  }])
}

resource "aws_ecs_task_definition" "odsmanager-high" {
  family = var.odsmanager_taskdefinition_name_high
  network_mode = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  cpu = 2048
  memory = 16384 
  execution_role_arn = aws_iam_role.odsmanager_ecs_task_execution_role.arn
  task_role_arn = aws_iam_role.odsmanager_ecs_task_role.arn
  container_definitions = jsonencode([{
    name = "${var.odsmanager_container_name}"
    image = "${aws_ecr_repository.theradex_odsmanager_dev.repository_url}:dev-latest"
    essential = true
    environment = [
      { 
        name = "ASPNETCORE_ENVIRONMENT"
        value = "Dev Non-Commercial" 
      },
      {
          "name": "ODSEnvironment",
          "value": var.odsmanager_env
      }
    ]
    portMappings = [
      {
        protocol = "tcp"
        containerPort = 80
        hostPort = 80
      },
      {
        protocol = "tcp"
        containerPort = 443
        hostPort = 443
      },
    ]
    logConfiguration: {
        logDriver: "awslogs",
        options: {
          awslogs-group: "/ecs/task/${var.odsmanager_taskdefinition_cloudwatch_group_name}",
          awslogs-region: var.odsmanager_region,         
          awslogs-stream-prefix: "ecs"
        }
      }
  }])
}

resource "aws_cloudwatch_log_group" "theradex_odsmanager_taskdefinition_cloudwatch_log_group" {
  name = "/ecs/task/${var.odsmanager_taskdefinition_cloudwatch_group_name}"
  tags = {
    Environment = var.odsmanager_env
  }
  retention_in_days = var.odsmanager_logs_retention_in_days
}

resource "aws_security_group" "theradex_app_odsmanager_sg" {
  name        = "odsmanager-sg"
  description = "Allow TLS inbound traffic"
  vpc_id      = module.dev_noncomm_vpc.id

  ingress {
    description      = "HTTPS from all accounts"
    from_port        = 443
    to_port          = 443
    protocol         = "tcp"
    cidr_blocks      = ["10.0.0.0/10"]
  }

  ingress {
    description      = "HTTP from all accounts"
    from_port        = 80 
    to_port          = 80 
    protocol         = "tcp"
    cidr_blocks      = ["10.0.0.0/10"]
  }

  egress {
    from_port        = 0
    to_port          = 0
    protocol         = "-1"
    cidr_blocks      = ["0.0.0.0/0"]
    ipv6_cidr_blocks = ["::/0"]
  }

  tags = {
    Name = "allow_tls"
  }
}