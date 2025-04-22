data "aws_caller_identity" "current" {}
data "aws_region" "current" {}
data "aws_iam_policy_document" "default_policy" {
  statement {
    sid = "AWSCloudTrailAclCheck"

    principals {
      type        = "Service"
      identifiers = ["cloudtrail.amazonaws.com"]
    }

    actions = [
      "s3:GetBucketAcl"
    ]

    resources = [
      "arn:aws:s3:::${var.bucket_name}",
    ]
  }

  statement {
    sid = "AWSCloudTrailWrite"

    principals {
      type        = "Service"
      identifiers = ["cloudtrail.amazonaws.com"]
    }

    actions = [
      "s3:PutObject"
    ]

    resources = [
      "arn:aws:s3:::${var.bucket_name}/AWSLogs/*",
    ]

    condition {
      test     = "StringEquals"
      variable = "s3:x-amz-acl"
      values   = ["bucket-owner-full-control"]
    }
  }
}

data "aws_iam_policy_document" "deny_delete_policy" {
  source_policy_documents = [data.aws_iam_policy_document.default_policy.json]
  statement {
    sid = "DenyDeleteObjects"

    effect = "Deny"

    principals {
      type        = "AWS"
      identifiers = ["*"]
    }

    actions = [
      "s3:DeleteObject",
      "s3:DeleteObjectVersion"
    ]

    resources = [
      "arn:aws:s3:::${var.bucket_name}/*"
    ]
  }
}

locals {
  # Generate list of bucket ARNs based on possible values of var.event_source_buckets
  data_event_bucket_arns = length(
  var.event_source_buckets) == 1 && element(concat(var.event_source_buckets, [""]), 0) == "*" ? ["arn:aws:s3:::"] : [for b in var.event_source_buckets : "arn:aws:s3:::${b}/"]
}

resource "aws_cloudtrail" "trail" {
  name                          = var.name
  s3_bucket_name                = aws_s3_bucket.bucket.bucket
  s3_key_prefix                 = ""
  kms_key_id                    = aws_kms_key.cloudtrail.arn
  enable_log_file_validation    = var.enable_log_file_validation
  include_global_service_events = var.include_global_service_events
  is_multi_region_trail         = var.is_multi_region_trail
  is_organization_trail         = var.is_organization_trail
  cloud_watch_logs_role_arn     = aws_iam_role.cloudtrail.arn
  cloud_watch_logs_group_arn    = "${aws_cloudwatch_log_group.cloudtrail.arn}:*"

  dynamic "event_selector" {
    for_each = length(var.event_source_buckets) == 0 ? [] : [1]

    content {
      read_write_type           = "All"
      include_management_events = true

      data_resource {
        type   = "AWS::S3::Object"
        values = local.data_event_bucket_arns
      }
    }
  }

  tags = merge(var.tags, { "Name" = var.name })
}

resource "aws_s3_bucket" "bucket" {
  bucket = var.bucket_name

  tags = merge(var.tags, { "Name" = var.name })
}

resource "aws_s3_bucket_policy" "bucket" {
  bucket = aws_s3_bucket.bucket.id
  policy = var.deny_delete ? data.aws_iam_policy_document.deny_delete_policy.json : data.aws_iam_policy_document.default_policy.json
}

resource "aws_s3_bucket_acl" "bucket" {
  bucket = aws_s3_bucket.bucket.bucket 
  acl    = "private"
}

resource "aws_s3_bucket_public_access_block" "bucket" {
  bucket                  = aws_s3_bucket.bucket.id
  block_public_acls       = true
  block_public_policy     = true
  ignore_public_acls      = true
  restrict_public_buckets = true
}

resource "aws_cloudwatch_log_group" "cloudtrail" {
  name              = var.name
  retention_in_days = var.retention_in_days
  tags              = merge(var.tags, { "Name" = var.name })
}
