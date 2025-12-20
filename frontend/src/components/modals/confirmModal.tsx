"use client";

import { BaseModal } from "./baseModal";

interface ConfirmDialogProps {
  open: boolean;
  onClose: () => void;
  onConfirm: () => void;
  message: string;
}

export function ConfirmDialog({
  open,
  onClose,
  onConfirm,
  message,
}: ConfirmDialogProps) {
  return (
    <BaseModal
      open={open}
      onClose={onClose}
      title="Confirm Action"
      description={message}
      onConfirm={onConfirm}
      confirmLabel="Yes, Proceed"
    />
  );
}
