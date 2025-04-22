resource "aws_iam_account_password_policy" "password-policy" {
  count = var.password_policy_enabled ? 1 : 0

  minimum_password_length        = var.password_policy_minimum_length
  require_uppercase_characters   = var.password_policy_require_uppercase_characters
  require_lowercase_characters   = var.password_policy_require_lowercase_characters
  require_numbers                = var.password_policy_require_numbers
  require_symbols                = var.password_policy_require_symbols
  allow_users_to_change_password = var.password_policy_allow_users_to_change
  max_password_age               = var.password_policy_max_age
  password_reuse_prevention      = var.password_policy_reuse_prevention
}
