output "id" {
  description = "Instance ID"
  value       = aws_instance.ec2.id
}

output "private_ip" {
  description = "Private IP Address"
  value       = aws_instance.ec2.private_ip
}

output "public_ip" {
  description = "Public IP Address"
  value       = aws_instance.ec2.public_ip
}

output "security_groups" {
  description = "Attached security groups"
  value       = aws_instance.ec2.security_groups
}

output "subnet_id" {
  description = "Subnet ID"
  value       = aws_instance.ec2.subnet_id
}

# output "cloudwatch_alarm_arn_instance_fail" {
#   description = "ARN of CloudWatch Alarm for XXXXXXXX"
#   value       = aws_cloudwatch_metric_alarm.instance_status_check_failed.arn
# }

# output "cloudwatch_alarm_arn_system_fail" {
#   description = "ARN of CloudWatch Alarm for XXXXXXXX"
#   value       = aws_cloudwatch_metric_alarm.system_status_check_failed.arn
# }

# output "cloudwatch_alarm_id_instance_fail" {
#   description = "ID of CloudWatch Alarm for XXXXXXXX"
#   value       = aws_cloudwatch_metric_alarm.instance_status_check_failed.id
# }

# output "cloudwatch_alarm_id_system_fail" {
#   description = "ID of CloudWatch Alarm for XXXXXXXX"
#   value       = aws_cloudwatch_metric_alarm.system_status_check_failed.id
# }
