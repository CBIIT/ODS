# For new accounts copy this file to the new account's
# folder and only update the key value

terraform {

  required_version = ">= 1.8.4"

  backend "s3" {
    bucket         = "theradex-development-nci-terraform"
    key            = "dev/us-east-1/terraform.tfstate"
    kms_key_id     = "arn:aws:kms:us-east-1:993530973844:key/f671b797-e654-4378-87ab-277b10a50b77"
    region         = "us-east-1"
    encrypt        = true
    dynamodb_table = "theradex-development-nci-terraform"
    #profile        = "theradex-development-nci"
    # assume_role = {
    #   role_arn = "arn:aws:iam::993530973844:role/Terraform-oidc"
    # }
  }
}