resource "aws_guardduty_detector" "gd_detector" {
  enable                       = var.enable
  finding_publishing_frequency = var.finding_publishing_frequency
}

resource "aws_guardduty_member" "member" {
  for_each = var.invite_accounts

  account_id         = each.key
  email              = each.value
  detector_id        = aws_guardduty_detector.gd_detector.id
  invite             = true
  invitation_message = var.invite_message
}
