resource "aws_guardduty_detector" "member" {
  enable = var.enable
}

resource "aws_guardduty_invite_accepter" "member_from_master" {
  detector_id       = aws_guardduty_detector.member.id
  master_account_id = var.master_aws_account_id
}
