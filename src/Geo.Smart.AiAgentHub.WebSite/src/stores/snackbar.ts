import { useSnackbar } from '@smart/vue-snackbar';
import { defineStore } from 'pinia';

const useSnackbarStore = defineStore('snackbar', () => {
    const {
        snackbarModel,
        options,
        succSnack,
        errSnack,
        closeSnack,
    } = useSnackbar();

    return {
        snackbarModel,
        snackbarOptions: options,
        errSnack,
        succSnack,
        closeSnack,
    };
});

export default useSnackbarStore;
