import { useDialog } from '@smart/vue-dialog';
import { defineStore } from 'pinia';

const useDialogStore = defineStore('dialog', () => {
    const {
        dialog,
        setTitle,
        setContent,
        toggleDialog,
        setSubmitDialog,
        clearDialog,
        openDialog,
    } = useDialog();

    return {
        dialog,
        setTitle,
        setContent,
        toggleDialog,
        setSubmitDialog,
        clearDialog,
        openDialog,
    };
});

export default useDialogStore;
