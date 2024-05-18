import { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as Yup from 'yup';
import { useSelector, useDispatch } from 'react-redux';

import { history } from '_helpers';
import { linkActions, alertActions } from '_store';

export { AddEdit };

function AddEdit() {
    const { id } = useParams();
    const [title, setTitle] = useState();
    const dispatch = useDispatch();
    const link = useSelector(x => x.links?.item);

    // form validation rules 
    const validationSchema = Yup.object().shape({
        originalUrl: Yup.string().required('Original Url is required'),
    });
    const formOptions = { resolver: yupResolver(validationSchema) };

    // get functions to build form with useForm() hook
    const { register, handleSubmit, reset, formState } = useForm(formOptions);
    const { errors, isSubmitting } = formState;

    useEffect(() => {
        if (id) {
            setTitle('Edit Link');
            // fetch user details into redux state and 
            // populate form fields with reset()
            dispatch(linkActions.getById(id)).unwrap()
                .then(link => reset(link));
        } else {
            setTitle('Add Link');
        }
    }, []);

    async function onSubmit({originalUrl}) {
        dispatch(alertActions.clear());
        try {
            await dispatch(linkActions.addNewLink({originalUrl}));
            history.navigate('/links');
        } catch (error) {
            dispatch(alertActions.error(error));
        }
    }

    return (
        <>
            <h1>{title}</h1>
            {!(link?.loading || link?.error) &&
                <form onSubmit={handleSubmit(onSubmit)}>
                    <div className="row">
                        <div className="mb-3 col">
                            <label className="form-label">Original url</label>
                            <input name="originalUrl" type="text" {...register('originalUrl')} className={`form-control ${errors.originalUrl ? 'is-invalid' : ''}`} />
                            <div className="invalid-feedback">{errors.originalUrl?.message}</div>
                        </div>
                       
                    </div>
                    <div className="mb-3 col">
                        <button type="submit" disabled={isSubmitting} className="btn btn-primary me-2">
                            {isSubmitting && <span className="spinner-border spinner-border-sm me-1"></span>}
                            Save
                        </button>
                        <button onClick={() => reset()} type="button" disabled={isSubmitting} className="btn btn-secondary">Reset</button>
                        <Link to="/links" className="btn btn-link">Cancel</Link>
                    </div>
                </form>
            }
            {link?.loading &&
                <div className="text-center m-5">
                    <span className="spinner-border spinner-border-lg align-center"></span>
                </div>
            }
            {link?.error &&
                <div class="text-center m-5">
                    <div class="text-danger">Error loading link: {link.error}</div>
                </div>
            }
        </>
    );
}
