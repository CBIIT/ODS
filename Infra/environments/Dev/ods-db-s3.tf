#S3 Bucket for Development OARS-Development
resource "aws_s3_bucket" "ods-table-data" {
   bucket = var.nci-ods-s3-bucket-name
   tags             = var.tags

}

resource "aws_s3_bucket_public_access_block" "ods-table-data" {
   bucket = aws_s3_bucket.ods-table-data.id
   block_public_acls       = false
   block_public_policy     = false
   ignore_public_acls      = false
   restrict_public_buckets = false
}

