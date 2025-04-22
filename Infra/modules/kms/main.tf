data "aws_caller_identity" "current" {}
data "aws_region" "current" {}

data "aws_iam_policy_document" "ebs" {

  statement {
    sid = "Enable IAM User Permissions"

    actions   = ["kms:*"]
    effect    = "Allow"
    resources = ["*"]

    principals {
      type = "AWS"
      identifiers = compact([
        "arn:aws:iam::${data.aws_caller_identity.current.account_id}:root"
      ])
    }
  }

  statement {
    sid = "Enable IAM Autoscaling Service Role Permissions"

    actions = [
      "kms:Encrypt",
      "kms:Decrypt",
      "kms:ReEncryptFrom",
      "kms:ReEncryptTo",
      "kms:GenerateDataKey",
      "kms:GenerateDataKeyWithoutPlaintext",
      "kms:DescribeKey",
    ]
    effect    = "Allow"
    resources = ["*"]

    condition {
      test     = "StringEquals"
      variable = "kms:ViaService"
      values   = ["ec2.${data.aws_region.current.name}.amazonaws.com"]
    }

    principals {
      type = "AWS"
      identifiers = compact([
        "arn:aws:iam::${data.aws_caller_identity.current.account_id}:role/aws-service-role/autoscaling.amazonaws.com/AWSServiceRoleForAutoScaling",
        var.service_role_batch_arn,
        var.service_linked_role_spot_arn,
        var.service_role_spot_fleet_arn
      ])
    }

  }

  statement {
    sid = "Allow attachment of persistent resources"

    actions = [
      "kms:CreateGrant",
      "kms:ListGrants",
      "kms:RevokeGrant",
    ]
    effect    = "Allow"
    resources = ["*"]

    condition {
      test     = "Bool"
      variable = "kms:GrantIsForAWSResource"
      values   = ["true"]
    }

    principals {
      type = "AWS"
      identifiers = compact([
        "arn:aws:iam::${data.aws_caller_identity.current.account_id}:role/aws-service-role/autoscaling.amazonaws.com/AWSServiceRoleForAutoScaling",
        var.service_role_batch_arn,
        var.service_linked_role_spot_arn,
        var.service_role_spot_fleet_arn
      ])
    }
  }
}

resource "aws_kms_key" "backup" {
  count = var.backup ? 1 : 0

  description             = "backup"
  deletion_window_in_days = var.kms_key_deletion_window_in_days
  enable_key_rotation     = var.kms_key_rotation

  policy = <<POLICY
{
    "Version": "2012-10-17",
    "Id": "efs",
    "Statement": [
        {
          "Sid": "Enable IAM User Permissions",
          "Effect": "Allow",
          "Principal": {
              "AWS": "arn:aws:iam::${data.aws_caller_identity.current.account_id}:root"
          },
          "Action": "kms:*",
          "Resource": "*"
        }
    ]
}
POLICY

  tags = merge(var.tags, { "Name" = "backup" })
}

resource "aws_kms_alias" "backup" {
  count = var.backup ? 1 : 0

  name          = "alias/backup-${data.aws_region.current.name}"
  target_key_id = aws_kms_key.backup[count.index].key_id
}

resource "aws_kms_key" "dynamodb" {
  count                   = var.dynamodb ? 1 : 0
  description             = "dynamodb"
  deletion_window_in_days = var.kms_key_deletion_window_in_days
  enable_key_rotation     = var.kms_key_rotation

  policy = <<POLICY
{
    "Version": "2012-10-17",
    "Id": "dynamodb",
    "Statement": [
        {
          "Sid": "Enable IAM User Permissions",
          "Effect": "Allow",
          "Principal": {
              "AWS": "arn:aws:iam::${data.aws_caller_identity.current.account_id}:root"
          },
          "Action": "kms:*",
          "Resource": "*"
        }
    ]
}
POLICY
  tags   = merge(var.tags, { "Name" = "dynamodb" })
}

resource "aws_kms_alias" "dynamodb" {
  count         = var.dynamodb ? 1 : 0
  name          = "alias/dynamodb-${data.aws_region.current.name}"
  target_key_id = aws_kms_key.dynamodb[count.index].key_id
}

resource "aws_kms_key" "efs" {
  count = var.efs ? 1 : 0

  description             = "efs"
  deletion_window_in_days = var.kms_key_deletion_window_in_days
  enable_key_rotation     = var.kms_key_rotation

  policy = <<POLICY
{
    "Version": "2012-10-17",
    "Id": "efs",
    "Statement": [
        {
          "Sid": "Enable IAM User Permissions",
          "Effect": "Allow",
          "Principal": {
              "AWS": "arn:aws:iam::${data.aws_caller_identity.current.account_id}:root"
          },
          "Action": "kms:*",
          "Resource": "*"
        }
    ]
}
POLICY
  tags   = merge(var.tags, { "Name" = "efs" })
}

resource "aws_kms_alias" "efs" {
  count = var.efs ? 1 : 0

  name          = "alias/efs-${data.aws_region.current.name}"
  target_key_id = aws_kms_key.efs[count.index].key_id
}

resource "aws_kms_key" "ebs" {
  count                   = var.ebs ? 1 : 0
  description             = "ebs"
  deletion_window_in_days = var.kms_key_deletion_window_in_days
  enable_key_rotation     = var.kms_key_rotation
  policy                  = data.aws_iam_policy_document.ebs.json

  tags = merge(var.tags, { "Name" = "ebs" })
}

resource "aws_kms_alias" "ebs" {
  count         = var.ebs ? 1 : 0
  name          = "alias/ebs-${data.aws_region.current.name}"
  target_key_id = aws_kms_key.ebs[0].key_id
}

resource "aws_ebs_encryption_by_default" "ebs_encryption" {
  enabled = var.ebs_encryption_by_default
}

resource "aws_ebs_default_kms_key" "ebs_encryption" {
  count   = var.ebs_encryption_by_default && var.ebs ? 1 : 0
  key_arn = aws_kms_key.ebs[0].arn
}

resource "aws_kms_key" "rds" {
  count = var.rds ? 1 : 0

  description             = "rds"
  deletion_window_in_days = var.kms_key_deletion_window_in_days
  enable_key_rotation     = var.kms_key_rotation

  policy = <<POLICY
{
    "Version": "2012-10-17",
    "Id": "rds",
    "Statement": [
        {
            "Sid": "Enable IAM User Permissions",
            "Effect": "Allow",
            "Principal": {
                "AWS": "arn:aws:iam::${data.aws_caller_identity.current.account_id}:root"
            },
            "Action": "kms:*",
            "Resource": "*"
        }
    ]
}
POLICY
  tags   = merge(var.tags, { "Name" = "rds" })
}

resource "aws_kms_alias" "rds" {
  count = var.rds ? 1 : 0

  name          = "alias/rds-${data.aws_region.current.name}"
  target_key_id = aws_kms_key.rds[count.index].key_id
}

resource "aws_kms_key" "s3" {
  count = var.s3 ? 1 : 0

  description             = "s3"
  deletion_window_in_days = var.kms_key_deletion_window_in_days
  enable_key_rotation     = var.kms_key_rotation

  policy = <<POLICY
{
    "Version": "2012-10-17",
    "Id": "s3",
    "Statement": [
        {
          "Sid": "Enable IAM User Permissions",
          "Effect": "Allow",
          "Principal": {
              "AWS": "arn:aws:iam::${data.aws_caller_identity.current.account_id}:root"
          },
          "Action": "kms:*",
          "Resource": "*"
        }
    ]
}
POLICY
  tags   = merge(var.tags, { "Name" = "s3" })
}

resource "aws_kms_alias" "s3" {
  count         = var.s3 ? 1 : 0
  name          = "alias/s3-${data.aws_region.current.name}"
  target_key_id = aws_kms_key.s3[count.index].key_id
}

resource "aws_kms_key" "ssm" {
  count                   = var.ssm ? 1 : 0
  description             = "ssm"
  deletion_window_in_days = var.kms_key_deletion_window_in_days
  enable_key_rotation     = var.kms_key_rotation

  policy = <<POLICY
{
    "Version": "2012-10-17",
    "Id": "ssm",
    "Statement": [
        {
            "Sid": "Enable IAM User Permissions",
            "Effect": "Allow",
            "Principal": {
                "AWS": "arn:aws:iam::${data.aws_caller_identity.current.account_id}:root"
            },
            "Action": "kms:*",
            "Resource": "*"
        }
    ]
}
POLICY
  tags   = merge(var.tags, { "Name" = "ssm" })
}

resource "aws_kms_alias" "ssm" {
  count         = var.ssm ? 1 : 0
  name          = "alias/ssm-${data.aws_region.current.name}"
  target_key_id = aws_kms_key.ssm[count.index].key_id
}

resource "aws_kms_key" "sqs" {
  count = var.sqs ? 1 : 0

  description             = "sqs"
  deletion_window_in_days = var.kms_key_deletion_window_in_days
  enable_key_rotation     = var.kms_key_rotation

  policy = <<POLICY
{
    "Version": "2012-10-17",
    "Id": "sqs",
    "Statement": [
        {
          "Sid": "Enable IAM User Permissions",
          "Effect": "Allow",
          "Principal": {
              "AWS": "arn:aws:iam::${data.aws_caller_identity.current.account_id}:root"
          },
          "Action": "kms:*",
          "Resource": "*"
        }
    ]
}
POLICY
  tags   = merge(var.tags, { "Name" = "sqs" })
}

resource "aws_kms_alias" "sqs" {
  count         = var.sqs ? 1 : 0
  name          = "alias/sqs-${data.aws_region.current.name}"
  target_key_id = aws_kms_key.sqs[count.index].key_id
}

resource "aws_kms_key" "secretsmanager" {
  count                   = var.secretsmanager ? 1 : 0
  description             = "secretsmanager"
  deletion_window_in_days = var.kms_key_deletion_window_in_days
  enable_key_rotation     = var.kms_key_rotation

  policy = <<POLICY
{
    "Version": "2012-10-17",
    "Id": "secretsmanager",
    "Statement": [
        {
            "Sid": "Enable IAM User Permissions",
            "Effect": "Allow",
            "Principal": {
                "AWS": "arn:aws:iam::${data.aws_caller_identity.current.account_id}:root"
            },
            "Action": "kms:*",
            "Resource": "*"
        }
    ]
}
POLICY
  tags   = merge(var.tags, { "Name" = "secretsmanager" })
}

resource "aws_kms_key" "sns" {
  count                   = var.sns ? 1 : 0
  description             = "sns"
  deletion_window_in_days = var.kms_key_deletion_window_in_days
  enable_key_rotation     = var.kms_key_rotation
  policy                  = <<POLICY
{
    "Version": "2012-10-17",
    "Id": "sns",
    "Statement": [
        {
          "Sid": "Enable IAM User Permissions",
          "Effect": "Allow",
          "Principal": {
              "AWS": "arn:aws:iam::${data.aws_caller_identity.current.account_id}:root"
          },
          "Action": "kms:*",
          "Resource": "*"
        }
    ]
}
POLICY
  tags                    = merge(var.tags, { "Name" = "sns" })
}

resource "aws_kms_alias" "sns" {
  count         = var.sns ? 1 : 0
  name          = "alias/sns-${data.aws_region.current.name}"
  target_key_id = aws_kms_key.sns[count.index].key_id
}

resource "aws_kms_alias" "secretsmanager" {
  count         = var.secretsmanager ? 1 : 0
  name          = "alias/secretsmanager-${data.aws_region.current.name}"
  target_key_id = aws_kms_key.secretsmanager[count.index].key_id
}

resource "aws_kms_key" "elasticache" {
  count                   = var.elasticache ? 1 : 0
  description             = "elasticache"
  deletion_window_in_days = var.kms_key_deletion_window_in_days
  enable_key_rotation     = var.kms_key_rotation

  policy = <<POLICY
{
    "Version": "2012-10-17",
    "Id": "elasticache",
    "Statement": [
        {
            "Sid": "Enable IAM User Permissions",
            "Effect": "Allow",
            "Principal": {
                "AWS": "arn:aws:iam::${data.aws_caller_identity.current.account_id}:root"
            },
            "Action": "kms:*",
            "Resource": "*"
        }
    ]
}
POLICY
  tags   = merge(var.tags, { "Name" = "elasticache" })
}

resource "aws_kms_alias" "elasticache" {
  count         = var.elasticache ? 1 : 0
  name          = "alias/elasticache-${data.aws_region.current.name}"
  target_key_id = aws_kms_key.elasticache[count.index].key_id
}

resource "aws_kms_key" "workspaces" {
  count                   = var.workspaces ? 1 : 0
  description             = "workspaces"
  deletion_window_in_days = var.kms_key_deletion_window_in_days
  enable_key_rotation     = var.kms_key_rotation

  policy = <<POLICY
{
    "Version": "2012-10-17",
    "Id": "workspaces",
    "Statement": [
        {
            "Sid": "Enable IAM User Permissions",
            "Effect": "Allow",
            "Principal": {
                "AWS": "arn:aws:iam::${data.aws_caller_identity.current.account_id}:root"
            },
            "Action": "kms:*",
            "Resource": "*"
        }
    ]
}
POLICY
  tags   = merge(var.tags, { "Name" = "workspaces" })
}

resource "aws_kms_alias" "workspaces" {
  count         = var.workspaces ? 1 : 0
  name          = "alias/workspaces-${data.aws_region.current.name}"
  target_key_id = aws_kms_key.workspaces[count.index].key_id
}
