# This file defines the ECR Repos and Replication
# for containers: odsextractor

resource "aws_ecr_repository" "theradex_odsextractor_dev" {
  name = "dev-theradexodsextractor"
  image_tag_mutability = "MUTABLE"
  image_scanning_configuration {
    scan_on_push = true
  }
}

resource "aws_ecr_lifecycle_policy" "theradex_odsextractor_dev" {
  repository = aws_ecr_repository.theradex_odsextractor_dev.name
  policy = <<EOF
{
    "rules": [
        {
            "rulePriority": 1,
            "description": "Keep only 9 untagged images.",
            "selection": {
                "tagStatus": "untagged",
                "countType": "imageCountMoreThan",
                "countNumber": 9 
            },
            "action": {
                "type": "expire"
            }
        }
    ]
}
EOF
}

resource "aws_ecr_repository_policy" "theradex_odsextractor_dev_policy" {
  repository = aws_ecr_repository.theradex_odsextractor_dev.name
  policy = <<EOF
{
    "Version": "2008-10-17",
    "Statement": [
      {
        "Sid": "AllowPushPull",
        "Effect": "Allow",
        "Principal": {
          "AWS": "arn:aws:iam::${var.aws_accounts.theradex-shared-service.account_id}:root"
        },
        "Action": [
          "ecr:BatchCheckLayerAvailability",
          "ecr:BatchGetImage",
          "ecr:CompleteLayerUpload",
          "ecr:DescribeImages",
          "ecr:DescribeRepositories",
          "ecr:GetDownloadUrlForLayer",
          "ecr:InitiateLayerUpload",
          "ecr:PutImage",
          "ecr:UploadLayerPart"
        ],
        "Condition": {
          "ForAnyValue:StringLike": {
            "aws:PrincipalOrgPaths": [
              "${var.org-id}/*"
            ]
          }
        }
      }
    ]
}
EOF
}

resource "aws_ecr_registry_policy" "theradex_odsextractor_registry_policy" {
  policy = jsonencode({
    Version = "2012-10-17",
    Statement = [
      {
        Sid    = "AllowReplication",
        Effect = "Allow",
        Principal = {
          "AWS": "arn:aws:iam::${var.aws_accounts.theradex-shared-service.account_id}:root"
        },
        Action = [
          "ecr:ReplicateImage",
          "ecr:CreateRepository"
        ],
        Resource = [
          "*"
        ]
      }
    ]
  })
}

