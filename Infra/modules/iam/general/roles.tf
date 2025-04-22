resource "aws_iam_role" "ec2_ssm" {
  count       = var.role_ec2_ssm ? 1 : 0
  name        = "ec2-ssm"
  path        = "/"
  description = "Base EC2 role with access for managed services, such as SSM and Cloudwatch"

  assume_role_policy = <<POLICY
{
    "Version": "2012-10-17",
    "Statement": [
      {
        "Effect": "Allow",
        "Principal": {
          "Service": "ec2.amazonaws.com"
        },
        "Action": "sts:AssumeRole"
      }
    ]
}
POLICY

  tags = var.tags

  lifecycle {
    ignore_changes = [tags ["QSConfigId-079hl"]]
  }
}

resource "aws_iam_instance_profile" "ec2_ssm" {
  count = var.role_ec2_ssm ? 1 : 0

  name = aws_iam_role.ec2_ssm[0].name
  role = aws_iam_role.ec2_ssm[0].name
}

resource "aws_iam_role_policy_attachment" "ec2_ssm" {
  count = var.role_ec2_ssm ? 1 : 0

  role       = aws_iam_role.ec2_ssm[0].name
  policy_arn = "arn:aws:iam::aws:policy/AmazonSSMManagedInstanceCore"
}

resource "aws_iam_role_policy_attachment" "ec2_ssm_buckets" {
  count = var.role_ec2_ssm ? 1 : 0

  role       = aws_iam_role.ec2_ssm[0].name
  policy_arn = aws_iam_policy.ssm_buckets[0].arn
}

resource "aws_iam_role_policy_attachment" "ec2_cloudwatch" {
  count = var.role_ec2_ssm ? 1 : 0

  role       = aws_iam_role.ec2_ssm[0].name
  policy_arn = "arn:aws:iam::aws:policy/CloudWatchAgentServerPolicy"
}

resource "aws_iam_policy" "ssm_buckets" {
  count = var.role_ec2_ssm ? 1 : 0
  name  = "ssm-bucket-access"

  policy = <<POLICY
{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Sid": "amazonOwnedS3Buckets",
            "Effect": "Allow",
            "Action": "s3:GetObject",
            "Resource": [
                "arn:aws:s3:::aws-ssm-*/*",
                "arn:aws:s3:::aws-patch-manager-*/*",
                "arn:aws:s3:::aws-patchmanager-macos-*/*",
                "arn:aws:s3:::aws-windows-downloads-*/*",
                "arn:aws:s3:::amazon-ssm-*/*",
                "arn:aws:s3:::amazon-ssm-packages-*/*",
                "arn:aws:s3:::*-birdwatcher-prod/*",
                "arn:aws:s3:::patch-baseline-snapshot-*/*"
            ]
        }

    ]
}
POLICY

}

resource "aws_iam_role" "codepipeline" {
  count       = var.role_codepipeline ? 1 : 0
  name        = "codepipeline-service-role"
  path        = "/"
  description = "AWS CodePipeline Service Role"

  assume_role_policy = <<POLICY
{
    "Version": "2012-10-17",
    "Statement": [
      {
        "Effect": "Allow",
        "Principal": {
          "Service": "codepipeline.amazonaws.com"
        },
        "Action": "sts:AssumeRole"
      }
    ]
}
POLICY

  tags = var.tags
}

resource "aws_iam_role_policy" "codepipeline" {
  count = var.role_codepipeline ? 1 : 0
  name  = "codepipeline-service"
  role  = aws_iam_role.codepipeline[0].id

  policy = <<POLICY
{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Action": [
                "autoscaling:*",
                "cloudformation:*",
                "codebuild:BatchGetBuilds",
                "codebuild:StartBuild",
                "codecommit:CancelUploadArchive",
                "codecommit:GetBranch",
                "codecommit:GetCommit",
                "codecommit:GetUploadArchiveStatus",
                "codecommit:UploadArchive",
                "codedeploy:CreateDeployment",
                "codedeploy:GetApplication",
                "codedeploy:GetApplicationRevision",
                "codedeploy:GetDeployment",
                "codedeploy:GetDeploymentConfig",
                "codedeploy:RegisterApplicationRevision",
                "codestar-connections:UseConnection",
                "ec2:*",
                "ecr:DescribeImages",
                "ecs:*",
                "iam:PassRole",
                "lambda:InvokeFunction",
                "lambda:ListFunctions",
                "rds:*",
                "s3:*",
                "sns:*",
                "sqs:*"
            ],
            "Resource": "*",
            "Effect": "Allow"
        }
    ]
}
POLICY
}

resource "aws_iam_role_policy" "codepipeline_sts_cloudformation" {
  count = var.role_codepipeline && var.role_cloudformation ? 1 : 0
  name  = "codepipeline-sts-assume-role-cloudformation"
  role  = aws_iam_role.codepipeline[0].id

  policy = <<POLICY
{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Effect": "Allow",
            "Action": "sts:AssumeRole",
            "Resource": "${aws_iam_role.cloudformation[0].arn}"
        }
    ]
}
POLICY
}

resource "aws_iam_role" "cloudformation" {
  count       = var.role_cloudformation ? 1 : 0
  name        = "cloudformation-service-role"
  path        = "/"
  description = "AWS cloudformation Service Role"

  assume_role_policy = <<POLICY
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Principal": {
        "Service": ["cloudformation.amazonaws.com", "codepipeline.amazonaws.com"]
      },
      "Action": "sts:AssumeRole"
    }
  ]
}
POLICY

  tags = var.tags
}

resource "aws_iam_role_policy" "cloudformation" {
  count = var.role_cloudformation ? 1 : 0
  name  = "cloudformation-service"
  role  = aws_iam_role.cloudformation[0].id

  policy = <<POLICY
{
  "Version": "2012-10-17",
  "Statement": [
    {
        "Action": [
            "apigateway:*",
            "codedeploy:*",
            "lambda:*",
            "events:CreateEventBus",
            "events:DeleteEventBus",
            "events:DescribeRule",
            "events:PutRule",
            "events:DeleteRule",
            "events:RemoveTargets",
            "events:PutTargets",
            "cloudformation:CreateChangeSet",
            "cloudformation:DescribeStacks",
            "iam:GetRole",
            "iam:CreateRole",
            "iam:DeleteRole",
            "iam:PutRolePolicy",
            "iam:AttachRolePolicy",
            "iam:DeleteRolePolicy",
            "iam:DetachRolePolicy",
            "iam:PassRole",
            "s3:GetObject",
            "s3:GetObjectVersion",
            "s3:GetBucketVersioning"
        ],
        "Resource": "*",
        "Effect": "Allow"
    }
  ]
}
POLICY
}


resource "aws_iam_role" "codebuild" {
  count       = var.role_codebuild ? 1 : 0
  name        = "codebuild-service-role"
  path        = "/"
  description = "AWS codebuild Service Role"

  assume_role_policy = <<POLICY
{
    "Version": "2012-10-17",
    "Statement": [
      {
        "Effect": "Allow",
        "Principal": {
          "Service": "codebuild.amazonaws.com"
        },
        "Action": "sts:AssumeRole"
      }
    ]
}
POLICY

  tags = var.tags
}

resource "aws_iam_role_policy" "codebuild" {
  count = var.role_codebuild ? 1 : 0
  name  = "codebuild-service"
  role  = aws_iam_role.codebuild[0].id

  policy = <<POLICY
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Sid": "CloudWatchLogsPolicy",
      "Effect": "Allow",
      "Action": [
        "logs:CreateLogGroup",
        "logs:CreateLogStream",
        "logs:PutLogEvents"
      ],
      "Resource": "*"
    },
    {
      "Sid": "S3GetObjectPolicy",
      "Effect": "Allow",
      "Action": [
        "s3:GetObject",
        "s3:GetObjectVersion"
      ],
      "Resource": "*"
    },
    {
      "Sid": "S3PutObjectPolicy",
      "Effect": "Allow",
      "Action": [
        "s3:PutObject"
      ],
      "Resource": "*"
    },
    {
      "Sid": "ECRPullPolicy",
      "Effect": "Allow",
      "Action": [
        "ecr:BatchCheckLayerAvailability",
        "ecr:GetDownloadUrlForLayer",
        "ecr:BatchGetImage"
      ],
      "Resource": "*"
    },
    {
      "Sid": "ECRAuthPolicy",
      "Effect": "Allow",
      "Action": [
        "ecr:GetAuthorizationToken"
      ],
      "Resource": "*"
    },
    {
      "Sid": "S3BucketIdentity",
      "Effect": "Allow",
      "Action": [
        "s3:GetBucketAcl",
        "s3:GetBucketLocation"
      ],
      "Resource": "*"
    }
  ]
}
POLICY
}

resource "aws_iam_role" "batch" {
  count = var.role_batch ? 1 : 0
  name  = "batch-service-role"

  assume_role_policy = <<POLICY
{
    "Version": "2012-10-17",
    "Statement": [
    {
        "Action": "sts:AssumeRole",
        "Effect": "Allow",
        "Principal": {
        "Service": "batch.amazonaws.com"
        }
    }
    ]
}
POLICY
}

resource "aws_iam_role_policy_attachment" "batch" {
  count      = var.role_batch ? 1 : 0
  role       = aws_iam_role.batch[0].name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AWSBatchServiceRole"
}

resource "aws_iam_role" "batch_instance" {
  count = var.role_batch ? 1 : 0
  name  = "batch-instance"

  assume_role_policy = <<POLICY
{
    "Version": "2012-10-17",
    "Statement": [
    {
        "Action": "sts:AssumeRole",
        "Effect": "Allow",
        "Principal": {
        "Service": "ec2.amazonaws.com"
        }
    }
    ]
}
POLICY
}

resource "aws_iam_instance_profile" "batch_instance" {
  count = var.role_batch ? 1 : 0

  name = aws_iam_role.batch_instance[0].name
  role = aws_iam_role.batch_instance[0].name
}

resource "aws_iam_role_policy_attachment" "batch_instance_ssm" {
  count      = var.role_batch ? 1 : 0
  role       = aws_iam_role.batch_instance[0].name
  policy_arn = "arn:aws:iam::aws:policy/AmazonSSMManagedInstanceCore"
}

resource "aws_iam_role_policy_attachment" "batch_instance_container_service" {
  count      = var.role_batch ? 1 : 0
  role       = aws_iam_role.batch_instance[0].name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AmazonEC2ContainerServiceforEC2Role"
}

resource "aws_iam_role_policy" "codebuild_ecr_push" {
  count = var.role_codebuild && var.role_codebuild_ecr_push ? 1 : 0
  name  = "codebuild-service-ecs-push"
  role  = aws_iam_role.codebuild[0].id

  policy = <<POLICY
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "ecr:CompleteLayerUpload",
        "ecr:GetAuthorizationToken",
        "ecr:UploadLayerPart",
        "ecr:InitiateLayerUpload",
        "ecr:BatchCheckLayerAvailability",
        "ecr:PutImage"
      ],
      "Resource": "*"
    }
  ]
}
POLICY
}

resource "aws_iam_role" "spot_fleet" {
  count = var.role_spot_fleet ? 1 : 0
  name  = "spot-fleet-service-role"

  assume_role_policy = <<POLICY
{
    "Version": "2012-10-17",
    "Statement": [
    {
        "Action": "sts:AssumeRole",
        "Effect": "Allow",
        "Principal": {
        "Service": "spotfleet.amazonaws.com"
        }
    }
    ]
}
POLICY
}

resource "aws_iam_role_policy_attachment" "spot_fleet" {
  count      = var.role_spot_fleet ? 1 : 0
  role       = aws_iam_role.spot_fleet[0].name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AmazonEC2SpotFleetTaggingRole"
}

resource "aws_iam_role" "aws_backup" {
  count = var.role_aws_backup ? 1 : 0
  name  = "aws-backup-service-role"

  assume_role_policy = <<POLICY
{
    "Version": "2012-10-17",
    "Statement": [
    {
        "Action": "sts:AssumeRole",
        "Effect": "Allow",
        "Principal": {
        "Service": "backup.amazonaws.com"
        }
    }
    ]
}
POLICY
}

resource "aws_iam_role_policy_attachment" "aws_backup" {
  count      = var.role_aws_backup ? 1 : 0
  role       = aws_iam_role.aws_backup[0].name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AWSBackupServiceRolePolicyForBackup"
}


resource "aws_iam_role" "dlm" {
  count = var.role_dlm ? 1 : 0
  name  = "dlm-service-role"

  assume_role_policy = <<POLICY
{
    "Version": "2012-10-17",
    "Statement": [
    {
        "Action": "sts:AssumeRole",
        "Effect": "Allow",
        "Principal": {
        "Service": "dlm.amazonaws.com"
        }
    }
    ]
}
POLICY
}
