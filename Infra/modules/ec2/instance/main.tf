
data "aws_caller_identity" "current" {}
data "aws_region" "current" {}

resource "aws_instance" "ec2" {
  ami                         = var.ami
  associate_public_ip_address = var.associate_public_ip_address
  disable_api_termination     = var.disable_api_termination


  dynamic "ebs_block_device" {
    for_each = var.ebs_block_device
    content {
      delete_on_termination = lookup(ebs_block_device.value, "delete_on_termination", null)
      device_name           = ebs_block_device.value.device_name
      encrypted             = lookup(ebs_block_device.value, "encrypted", null)
      kms_key_id            = lookup(ebs_block_device.value, "kms_key_id", null)
      volume_size           = lookup(ebs_block_device.value, "volume_size", null)
      volume_type           = lookup(ebs_block_device.value, "volume_type", null)
    }
  }

  iam_instance_profile = var.create_role == true ? aws_iam_instance_profile.ec2_role[0].name : var.iam_instance_profile
  instance_type        = var.instance_type
  key_name             = var.key_name
  monitoring           = var.enable_detailed_monitoring
  private_ip           = var.private_ip

  dynamic "root_block_device" {
    for_each = var.root_block_device
    content {
      delete_on_termination = lookup(root_block_device.value, "delete_on_termination", null)
      encrypted             = lookup(root_block_device.value, "encrypted", null)
      iops                  = lookup(root_block_device.value, "iops", null)
      kms_key_id            = lookup(root_block_device.value, "kms_key_id", null)
      volume_size           = lookup(root_block_device.value, "volume_size", null)
      volume_type           = lookup(root_block_device.value, "volume_type", null)
    }
  }

  source_dest_check      = var.source_dest_check
  subnet_id              = var.subnet_id
  tags                   = var.tags
  user_data              = var.user_data
  volume_tags            = var.volume_tags
  vpc_security_group_ids = var.vpc_security_group_ids
}
