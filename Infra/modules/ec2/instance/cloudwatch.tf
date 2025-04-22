resource "aws_cloudwatch_metric_alarm" "system_status_check_failed" {
  count               = var.system_status_check ? 1 : 0
  alarm_actions       = ["arn:aws:automate:${data.aws_region.current.name}:ec2:reboot"]
  alarm_description   = "Trigger a recovery when an underlying system status check fails for 3 consecutive minutes."
  alarm_name          = "${var.name}-system-check-failure"
  comparison_operator = "GreaterThanThreshold"
  evaluation_periods  = "3"
  metric_name         = "StatusCheckFailed_System"
  namespace           = "AWS/EC2"
  period              = "60"
  statistic           = "Minimum"
  threshold           = "0"
  dimensions = {
    InstanceId = aws_instance.ec2.id
  }
}

resource "aws_cloudwatch_metric_alarm" "instance_status_check_failed" {
  count               = var.instance_status_check ? 1 : 0
  alarm_actions       = ["arn:aws:automate:${data.aws_region.current.name}:ec2:reboot"]
  alarm_description   = "Trigger a reboot when instance status check fails for 3 consecutive minutes."
  alarm_name          = "${var.name}-instance-check-failure"
  comparison_operator = "GreaterThanThreshold"
  evaluation_periods  = "3"
  metric_name         = "StatusCheckFailed_Instance"
  namespace           = "AWS/EC2"
  period              = "60"
  statistic           = "Minimum"
  threshold           = "0"
  dimensions = {
    InstanceId = aws_instance.ec2.id
  }
}

resource "aws_cloudwatch_metric_alarm" "cpu_utilization" {
  count               = var.cpu_utilization ? 1 : 0
  alarm_actions       = ["arn:aws:sns:${data.aws_region.current.name}:${data.aws_caller_identity.current.account_id}:${var.alarm_sns_topic_name}"]
  alarm_description   = "Send notification to sns."
  alarm_name          = "${var.name}-cpu-utilization"
  comparison_operator = "GreaterThanThreshold"
  evaluation_periods  = var.cpu_eval_period
  metric_name         = "CPUUtilization"
  namespace           = "AWS/EC2"
  period              = var.cpu_eval_period_length
  statistic           = "Minimum"
  threshold           = var.cpu_threshold
  dimensions = {
    InstanceId = aws_instance.ec2.id
  }
}

resource "aws_cloudwatch_metric_alarm" "mem_utilization" {
  count               = var.mem_utilization ? 1 : 0
  alarm_actions       = ["arn:aws:sns:${data.aws_region.current.name}:${data.aws_caller_identity.current.account_id}:${var.alarm_sns_topic_name}"]
  alarm_description   = "Send notification to sns."
  alarm_name          = "${var.name}-mem-utilization"
  comparison_operator = "GreaterThanThreshold"
  evaluation_periods  = var.mem_eval_period
  metric_name         = "mem_used_percent"
  namespace           = "AWS/EC2"
  period              = var.mem_eval_period_length
  statistic           = "Minimum"
  threshold           = var.mem_threshold
  dimensions = {
    InstanceId = aws_instance.ec2.id
  }
}

resource "aws_cloudwatch_metric_alarm" "disk_utilization" {
  count               = var.disk_utilization ? 1 : 0
  alarm_actions       = ["arn:aws:sns:${data.aws_region.current.name}:${data.aws_caller_identity.current.account_id}:${var.alarm_sns_topic_name}"]
  alarm_description   = "Send notification to sns."
  alarm_name          = "${var.name}-disk-utilization"
  comparison_operator = "GreaterThanThreshold"
  evaluation_periods  = var.disk_eval_period
  metric_name         = "LogicalDisk % Free Space"
  namespace           = "AWS/EC2"
  period              = var.disk_eval_period_length
  statistic           = "Minimum"
  threshold           = var.disk_threshold
  dimensions = {
    InstanceId = aws_instance.ec2.id
  }
}
