"use client";

import { Dialog, DialogContent, DialogHeader, DialogFooter } from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";

interface BaseModalProps {
  open: boolean;
  onClose: () => void;
  title: string;
  description?: string;
  children?: React.ReactNode;
  onConfirm?: () => void;
  confirmLabel?: string;
}

export function BaseModal({ open, onClose, title, description, children, onConfirm, confirmLabel = "Confirm" }: BaseModalProps) {
  return (
    <Dialog open={open} onOpenChange={onClose}>
      <DialogContent>
        <DialogHeader>
          <h2>{title}</h2>
          {description && <p>{description}</p>}
        </DialogHeader>

        <div>{children}</div>

        <DialogFooter>
          <Button variant="outline" onClick={onClose}>Close</Button>
          {onConfirm && <Button onClick={onConfirm} className="ml-2">{confirmLabel}</Button>}
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}

export function ConfirmDialog({ open, onClose, onConfirm, message }: { open: boolean; onClose: () => void; onConfirm: () => void; message: string }) {
  return (
    <BaseModal open={open} onClose={onClose} title="Confirm Action" description={message} onConfirm={onConfirm} confirmLabel="Yes, Proceed" />
  );
}
