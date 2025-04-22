output "detector_id" {
  value = aws_guardduty_detector.gd_detector.id
}

output "aws_account_id" {
  value = aws_guardduty_detector.gd_detector.account_id
}
