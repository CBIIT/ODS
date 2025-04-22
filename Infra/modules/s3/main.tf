resource "aws_s3_bucket" "default" {
  bucket = var.name
  policy = var.policy

  dynamic "logging" {
    for_each = var.access_log_configuration
    content {
      target_bucket = lookup(logging.value, "target_bucket", null)
      target_prefix = lookup(logging.value, "target_prefix", null)
    }
  }

  # Credit - https://github.com/terraform-aws-modules/terraform-aws-s3-bucket/blob/master/main.tf
  dynamic "lifecycle_rule" {
    for_each = var.lifecycle_rule
    content {
      id                                     = lookup(lifecycle_rule.value, "id", null)
      prefix                                 = lookup(lifecycle_rule.value, "prefix", null)
      tags                                   = lookup(lifecycle_rule.value, "tags", null)
      enabled                                = lookup(lifecycle_rule.value, "enabled", null)
      abort_incomplete_multipart_upload_days = lookup(lifecycle_rule.value, "abort_incomplete_multipart_upload_days", null)

      # Max 1 block - expiration
      dynamic "expiration" {
        for_each = length(keys(lookup(lifecycle_rule.value, "expiration", {}))) == 0 ? [] : [lookup(lifecycle_rule.value, "expiration", {})]

        content {
          date                         = lookup(expiration.value, "date", null)
          days                         = lookup(expiration.value, "days", null)
          expired_object_delete_marker = lookup(expiration.value, "expired_object_delete_marker", null)
        }
      }

      # Several blocks - transition
      dynamic "transition" {
        for_each = lookup(lifecycle_rule.value, "transition", [])

        content {
          date          = lookup(transition.value, "date", null)
          days          = lookup(transition.value, "days", null)
          storage_class = transition.value.storage_class
        }
      }

      # Max 1 block - noncurrent_version_expiration
      dynamic "noncurrent_version_expiration" {
        for_each = length(keys(lookup(lifecycle_rule.value, "noncurrent_version_expiration", {}))) == 0 ? [] : [lookup(lifecycle_rule.value, "noncurrent_version_expiration", {})]

        content {
          days = lookup(noncurrent_version_expiration.value, "days", null)
        }
      }

      # Several blocks - noncurrent_version_transition
      dynamic "noncurrent_version_transition" {
        for_each = lookup(lifecycle_rule.value, "noncurrent_version_transition", [])

        content {
          days          = lookup(noncurrent_version_transition.value, "days", null)
          storage_class = noncurrent_version_transition.value.storage_class
        }
      }
    }
  }

  tags = var.tags
}

resource "aws_s3_bucket_public_access_block" "default" {
  bucket = aws_s3_bucket.default.id

  block_public_acls       = var.block_all_public_access
  block_public_policy     = var.block_all_public_access
  ignore_public_acls      = var.block_all_public_access
  restrict_public_buckets = var.block_all_public_access
}


resource "aws_s3_bucket_acl" "default" {
  bucket = aws_s3_bucket.default.bucket 
  acl    = var.acl
}

resource "aws_s3_bucket_server_side_encryption_configuration" "default" {
  bucket = aws_s3_bucket.default.bucket
    rule {
      apply_server_side_encryption_by_default {
        kms_master_key_id = var.kms_key_arn != "" ? var.kms_key_arn : null
        sse_algorithm     = var.kms_key_arn == "" ? "AES256" : "aws:kms"
      }
  }
}

resource "aws_s3_bucket_versioning" "default" {
  bucket = aws_s3_bucket.default.bucket
  versioning_configuration {
    status = var.versioning
  }
}