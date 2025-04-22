variable "provider_name" {
  type        = string
  description = "The name of your provider, which could be case sensitive (okta, azuread, gsuite, etc)"
}

variable "provider_metadata_file" {
  type        = string
  description = "The local path to the provider metadata xml file"
}

variable "create_user" {
  type        = bool
  description = "Flag to create user for listing roles (not needed for gsuite)"
}