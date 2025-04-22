resource "aws_s3_bucket" "s3bucket_odsextractor_files" {
    bucket = "${var.odsextractor_s3_bucket}"
}