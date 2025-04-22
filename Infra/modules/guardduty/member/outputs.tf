output "detector_id" {
  value = aws_guardduty_detector.member.id
}

output "aws_account_id" {
  value = aws_guardduty_detector.member.account_id
}
