variable "password_policy_allow_users_to_change" {
  description = "Allow users to change their own password"
  type        = bool
  default     = true
}

variable "password_policy_enabled" {
  description = "Create the password policy"
  type        = bool
  default     = true
}

variable "password_policy_max_age" {
  description = "Passwords are expired after this many days"
  type        = number
  default     = 90
}

variable "password_policy_minimum_length" {
  description = "Minimum length"
  type        = number
  default     = 14
}

variable "password_policy_require_numbers" {
  description = "Passwords must contain a number"
  type        = bool
  default     = true
}

variable "password_policy_require_lowercase_characters" {
  description = "Passwords must contain a lowercase character"
  type        = bool
  default     = true
}

variable "password_policy_require_uppercase_characters" {
  description = "Passwords must contain an uppercase character"
  type        = bool
  default     = true
}

variable "password_policy_require_symbols" {
  description = "Password must contain a symbol"
  type        = bool
  default     = true
}

variable "password_policy_reuse_prevention" {
  description = "Prevent using this number of previous passwords"
  type        = number
  default     = 24
}

variable "role_batch" {
  description = "Create batch service role and batch EC2 instance role"
  default     = false
  type        = bool
}

variable "role_codebuild" {
  description = "Create codebuild service role"
  default     = false
  type        = bool
}

variable "role_cloudformation" {
  description = "Create cloudformation service role"
  default     = false
  type        = bool
}

variable "role_ec2_ssm" {
  description = "Create role with SSM core policy attached"
  default     = true
  type        = bool
}

variable "role_codepipeline" {
  description = "Create codepipeline service role"
  default     = false
  type        = bool
}

variable "role_codebuild_ecr_push" {
  description = "Allow the codebuild service role to push to ECR repositories"
  default     = false
  type        = bool
}

variable "role_spot_fleet" {
  description = "Create spot fleet service role"
  default     = false
  type        = bool
}

variable "role_aws_backup" {
  description = "Create aws backup service role"
  default     = true
  type        = bool
}

variable "role_dlm" {
  description = "Create ebs data lifecycle manager service role"
  default     = true
  type        = bool
}

variable "tags" {
  description = "Tags to apply to all module resources."
  default     = {}
  type        = map(any)
}