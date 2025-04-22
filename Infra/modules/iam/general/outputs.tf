output "profile_ec2_ssm_arn" {
  description = "ARN of the ec2-ssm instance profile"
  value       = var.role_ec2_ssm ? aws_iam_instance_profile.ec2_ssm[0].arn : null
}

output "role_ec2_ssm_name" {
  description = "Name of the ec2-ssm role"
  value       = var.role_ec2_ssm ? aws_iam_role.ec2_ssm[0].name : null
}

output "policy_ssm_buckets_arn" {
  description = "Policy arn for ssm buckets permissions"
  value       = var.role_ec2_ssm ? aws_iam_policy.ssm_buckets[0].arn : null
}

output "service_linked_role_autoscaling_arn" {
  description = "ARN of the autoscaling service linked role"
  value       = aws_iam_service_linked_role.autoscaling.arn
}

output "service_linked_role_rds_arn" {
  description = "ARN of the RDS service linked role"
  value       = aws_iam_service_linked_role.rds.arn
}

output "service_linked_role_spot_arn" {
  description = "ARN of the Spot service linked role"
  value       = aws_iam_service_linked_role.spot.arn
}
output "service_role_backup_arn" {
  description = "ARN of the AWS Backup service role"
  value       = var.role_aws_backup ? aws_iam_role.aws_backup[0].arn : null
}
output "service_role_batch_arn" {
  description = "ARN of the batch service role"
  value       = var.role_batch ? aws_iam_role.batch[0].arn : null
}

output "role_batch_instance_arn" {
  description = "ARN of the batch EC2 instance role"
  value       = var.role_batch ? aws_iam_role.batch_instance[0].arn : null
}

output "profile_batch_instance_arn" {
  description = "ARN of the batch EC2 instance profile"
  value       = var.role_batch ? aws_iam_instance_profile.batch_instance[0].arn : null
}

output "service_role_codebuild_arn" {
  description = "ARN of the CodeBuild service role"
  value       = var.role_codebuild ? aws_iam_role.codebuild[0].arn : null
}

output "service_role_codebuild_name" {
  description = "Name of the CodeBuild service role"
  value       = var.role_codebuild ? aws_iam_role.codebuild[0].name : null
}

output "service_role_cloudformation_arn" {
  description = "ARN of the Cloudformation service role"
  value       = var.role_cloudformation ? aws_iam_role.cloudformation[0].arn : null
}

output "service_role_cloudformation_name" {
  description = "Name of the Cloudformation service role"
  value       = var.role_cloudformation ? aws_iam_role.cloudformation[0].name : null
}

output "service_role_codepipeline_arn" {
  description = "ARN of the CodePipeline service role"
  value       = var.role_codepipeline ? aws_iam_role.codepipeline[0].arn : null
}

output "service_role_codepipeline_name" {
  description = "Name of the CodePipeline service role"
  value       = var.role_codepipeline ? aws_iam_role.codepipeline[0].name : null
}

output "service_role_spot_fleet_arn" {
  description = "ARN of the spot fleet service role"
  value       = var.role_spot_fleet ? aws_iam_role.spot_fleet[0].arn : null
}