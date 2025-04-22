output "arn" {
  description = "Bucket ARN"
  value       = aws_s3_bucket.default.arn
}

output "hosted_zone_id" {
  description = "Canonical hosted zone ID of the load balancer.  Used for Route 53 entries."
  value       = aws_s3_bucket.default.hosted_zone_id
}

output "name" {
  description = "Bucket Name/ID"
  value       = aws_s3_bucket.default.id
}

output "region" {
  description = "Bucket region"
  value       = aws_s3_bucket.default.region
}
