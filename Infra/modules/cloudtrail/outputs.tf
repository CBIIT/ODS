output "bucket_arn" {
  description = "CloudTrail S3 bucket ARN"
  value       = aws_s3_bucket.bucket.arn
}

output "bucket_name" {
  description = "CloudTrail S3 bucket name"
  value       = aws_s3_bucket.bucket.id
}

output "trail_arn" {
  description = "CloudTrail Trail ARN"
  value       = aws_cloudtrail.trail.arn
}

output "trail_name" {
  description = "CloudTrail Trail name"
  value       = aws_cloudtrail.trail.id
}